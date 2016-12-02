#Ivy
Ivy is a small parametric design tool to explore simple system composed of :
- a doubly-cruved shell
- two linear actuators

This tool is embeded in a **Rhino & Grasshopper** plugin and allows to :
- define a topology (shell + 2 actuators)
- define a discrete grid of shapes that conform the given topology
- define a discrete grid of actuation (extension/shrining of actuators)

Deformations and stresses are computed for each grid node (both topology and actuation space) via Abaqus.
Models are launched with a combination of `.inp` and `.py` files.
Results are stored in a `.db` file via SQLite.
Results can be imported back into **Rhino & Grasshopper** for further benchmarking and analysis.

Mainly, this tool is provided to compare environmental performances of shading devices and find some optimal solutions of efficient shading.

##Authors
Lionel du Peloux - 2016
@PrincetonUniversity - USA
@Navier - France
