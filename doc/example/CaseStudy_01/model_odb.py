
from part import *
from material import *
from section import *
from assembly import *
from step import *
from interaction import *
from load import *
from mesh import *
from optimization import *
from job import *
from sketch import *
from visualization import *
from connectorBehavior import *

import sqlite3, inspect, os, sys

def GetSubdirectories(dir):
    return [name for name in os.listdir(dir)
	    if os.path.isdir(os.path.join(dir, name))]

# WORKING DIRECTORY
wkdir = 'C:\Users\Lionel\Documents\Abaqus\WorkingDir'
wkdir = os.path.dirname(os.path.abspath(inspect.getfile(inspect.currentframe())))



# OPEN DATA BASE
conn = sqlite3.connect(wkdir + '\\' + 'data.db')
c = conn.cursor()

# CLEAR EXISTING results
sql = 'DELETE FROM FIELD'
try:
    c.execute(sql)
except sqlite3.IntegrityError:
    print('ERROR : ATTEMPT TO EREASE ALL FIELD ENTRIES FAILED')

# SQL QUERRY TEMPLATE FOR INSERTING RESULTS
sql_insert_field = '''
                        INSERT INTO FIELD(ACT, SHP, NODE, X, Y, Z, DX, DY, DZ)
                        VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?)
                    '''

subdir_list = GetSubdirectories(wkdir)
numShapeNodes = len(subdir_list)

# GENERATE ABAQUS MODELS FROM INP FILES
for shapeNodeIndex in range(numShapeNodes):

    subdir = subdir_list[shapeNodeIndex]
    path = wkdir + '\\' + subdir
    os.chdir(path)
    print 'Processing Node : ' + subdir

    # OPEN ODB FILE
    odb = openOdb(path + '\\' + 'job.odb')

    # GET SHELL NODES
    nodeSet = odb.rootAssembly.instances['Instance A'].nodeSets['SHELL']
    numMeshNodes = len(nodeSet.nodes)

    # GET RESULTS
    numActuNodes = len(odb.steps)
    stepNames = odb.steps.keys()

    for actuNodeIndex in range(numActuNodes) :  
        # extract step name
        stepName = stepNames[actuNodeIndex]

        # extract frames for the given step
        numFrames = len(odb.steps[stepName].frames)

        lastFrame = odb.steps[stepName].frames[-1]
        field_Disp = lastFrame.fieldOutputs['U'].getSubset(region = nodeSet)

        for meshNodeIndex in range(numMeshNodes):

            field = field_Disp.values[meshNodeIndex]
            node = nodeSet.nodes[meshNodeIndex]

            values = (
                int(actuNodeIndex + 1),
                int(shapeNodeIndex + 1),
                int(meshNodeIndex + 1),
                float(node.coordinates[0]),
                float(node.coordinates[1]),
                float(node.coordinates[2]),
                float(field.data[0]),
                float(field.data[1]),
                float(field.data[2])
                )

            try:
				c.execute(sql_insert_field, values)            
            except sqlite3.IntegrityError:
                print('ERROR INSERTING FIELD ROW')
            
    conn.commit()
    odb.close()
            
conn.close()
            
            
