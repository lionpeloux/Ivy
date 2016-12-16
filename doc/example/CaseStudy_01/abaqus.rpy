# -*- coding: mbcs -*-
#
# Abaqus/CAE Release 6.12-1 replay file
# Internal Version: 2012_03_13-20.23.18 119612
# Run by Lionel on Fri Dec 16 10:52:41 2016
#

# from driverUtils import executeOnCaeGraphicsStartup
# executeOnCaeGraphicsStartup()
#: Executing "onCaeGraphicsStartup()" in the site directory ...
from abaqus import *
from abaqusConstants import *
session.Viewport(name='Viewport: 1', origin=(1.76563, 1.7625), width=259.9, 
    height=174.84)
session.viewports['Viewport: 1'].makeCurrent()
from driverUtils import executeOnCaeStartup
executeOnCaeStartup()
execfile('model_odb.py', __main__.__dict__)
#: Processing Node : 00
#: Model: C:/Users/Lionel/Documents/Abaqus/WorkingDir/00/job.odb
#: Number of Assemblies:         1
#: Number of Assembly instances: 0
#: Number of Part instances:     1
#: Number of Meshes:             1
#: Number of Element Sets:       184
#: Number of Node Sets:          21
#: Number of Steps:              16
#: Processing Node : 01
#: Model: C:/Users/Lionel/Documents/Abaqus/WorkingDir/01/job.odb
#: Number of Assemblies:         1
#: Number of Assembly instances: 0
#: Number of Part instances:     1
#: Number of Meshes:             1
#: Number of Element Sets:       184
#: Number of Node Sets:          21
#: Number of Steps:              16
#: Processing Node : 02
#: Model: C:/Users/Lionel/Documents/Abaqus/WorkingDir/02/job.odb
#: Number of Assemblies:         1
#: Number of Assembly instances: 0
#: Number of Part instances:     1
#: Number of Meshes:             1
#: Number of Element Sets:       184
#: Number of Node Sets:          21
#: Number of Steps:              16
#: Processing Node : 03
#: Model: C:/Users/Lionel/Documents/Abaqus/WorkingDir/03/job.odb
#: Number of Assemblies:         1
#: Number of Assembly instances: 0
#: Number of Part instances:     1
#: Number of Meshes:             1
#: Number of Element Sets:       184
#: Number of Node Sets:          21
#: Number of Steps:              16
#: Processing Node : 04
#: Model: C:/Users/Lionel/Documents/Abaqus/WorkingDir/04/job.odb
#: Number of Assemblies:         1
#: Number of Assembly instances: 0
#: Number of Part instances:     1
#: Number of Meshes:             1
#: Number of Element Sets:       184
#: Number of Node Sets:          21
#: Number of Steps:              16
#: Processing Node : 05
#: Model: C:/Users/Lionel/Documents/Abaqus/WorkingDir/05/job.odb
#: Number of Assemblies:         1
#: Number of Assembly instances: 0
#: Number of Part instances:     1
#: Number of Meshes:             1
#: Number of Element Sets:       184
#: Number of Node Sets:          21
#: Number of Steps:              16
#: Processing Node : 06
#: Model: C:/Users/Lionel/Documents/Abaqus/WorkingDir/06/job.odb
#: Number of Assemblies:         1
#: Number of Assembly instances: 0
#: Number of Part instances:     1
#: Number of Meshes:             1
#: Number of Element Sets:       184
#: Number of Node Sets:          21
#: Number of Steps:              16
#: Processing Node : 07
#: Model: C:/Users/Lionel/Documents/Abaqus/WorkingDir/07/job.odb
#: Number of Assemblies:         1
#: Number of Assembly instances: 0
#: Number of Part instances:     1
#: Number of Meshes:             1
#: Number of Element Sets:       184
#: Number of Node Sets:          21
#: Number of Steps:              16
#: Processing Node : 08
#: Model: C:/Users/Lionel/Documents/Abaqus/WorkingDir/08/job.odb
#: Number of Assemblies:         1
#: Number of Assembly instances: 0
#: Number of Part instances:     1
#: Number of Meshes:             1
#: Number of Element Sets:       184
#: Number of Node Sets:          21
#: Number of Steps:              16
print 'RT script done'
#: RT script done
