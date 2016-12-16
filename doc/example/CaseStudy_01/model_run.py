
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


# GENERATE ABAQUS MODELS FROM INP FILES
for subdir in GetSubdirectories(wkdir):

    # LOAD INP FILE
    path = wkdir + '\\' + subdir
    os.chdir(path)
    mdb = openMdb(path + '\\' + 'model.cae')
    print ' '
    print '-- ANALYSIS OF SHAPE : ' + str(subdir)
    mdb.jobs['job'].submit(consistencyChecking = OFF)
    mdb.jobs['job'].waitForCompletion()
    mdb.save()
    mdb.close()
            
