
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

  
# ACTUATION NODES
actu = [
	(-50.000, -50.000), 
	(-25.000, -50.000), 
	(0.000, -50.000), 
	(25.000, -50.000), 
	(-50.000, -25.000), 
	(-25.000, -25.000), 
	(0.000, -25.000), 
	(25.000, -25.000), 
	(-50.000, 0.000), 
	(-25.000, 0.000), 
	(0.000, 0.000), 
	(25.000, 0.000), 
	(-50.000, 25.000), 
	(-25.000, 25.000), 
	(0.000, 25.000), 
	(25.000, 25.000), 
	]
  
# STEP PROPERTIES
nlgeom = ON
maxNumInc = 200
initialInc = 0.01
minInc = 1E-06
maxInc = 0.05
  

# GENERATE ABAQUS MODELS FROM INP FILES
for subdir in GetSubdirectories(wkdir):

    # LOAD INP FILE
    path = wkdir + '\\' + subdir
    mdb.ModelFromInputFile(inputFileName = path + '\\' + 'model.inp', name = 'model')

    # DELETE ALL OTHER EXISTING MODELS
    for k in mdb.models.keys():
        if k <> 'model': del mdb.models[k]

    # CREATE FIELDS
    # T1 : actuation field 1 | +1K => 1mm expansion | -1K => 1mm skrinkage
    # T2 : actuation field 2 | +1K => 1mm expansion | -1K => 1mm skrinkage
    mdb.models['model'].Temperature(createStepName = 'Initial', crossSectionDistribution = CONSTANT_THROUGH_THICKNESS, distributionType = UNIFORM, magnitudes = (0.0, ), name = 'T1', region = mdb.models['model'].rootAssembly.instances['Instance A'].sets['A1'])
    mdb.models['model'].Temperature(createStepName = 'Initial', crossSectionDistribution = CONSTANT_THROUGH_THICKNESS, distributionType = UNIFORM, magnitudes = (0.0, ), name = 'T2', region = mdb.models['model'].rootAssembly.instances['Instance A'].sets['A2'])

    # CREATE STEPS
    stepName_prev = 'Initial'
    stepName = ''
    for i in range(len(actu)):
        a = actu[i]
        if i > 0: stepName_prev = stepName
        stepName = '(' + str(int(a[0])) + ',' + str(int(a[1])) + ')'
        mdb.models['model'].StaticStep(nlgeom = nlgeom, maxNumInc = maxNumInc, initialInc = initialInc, maxInc = maxInc, minInc = minInc, name = stepName, previous = stepName_prev)
        mdb.models['model'].predefinedFields['T1'].setValuesInStep(magnitudes = (a[0], ), stepName = stepName)
        mdb.models['model'].predefinedFields['T2'].setValuesInStep(magnitudes = (a[1], ), stepName = stepName)

    # CREATE NEW JOB
    mdb.Job(atTime = None, contactPrint = OFF, description = '', echoPrint = OFF, explicitPrecision = SINGLE, getMemoryFromAnalysis = True, historyPrint = OFF, memory = 90, memoryUnits = PERCENTAGE, model = 'model', modelPrint = OFF, multiprocessingMode = DEFAULT, name = 'job', nodalOutputPrecision = SINGLE, numCpus = 1, numGPUs = 0, queue = None, scratch = '', type = ANALYSIS, userSubroutine = '', waitHours = 0, waitMinutes = 0)

    # SAVE CAE MODEL
    mdb.saveAs(pathName = path + '\\' + 'model.cae')
    mdb.close()

