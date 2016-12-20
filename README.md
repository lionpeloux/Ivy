## What is Ivy

**Ivy** is a small toolbox that helps organizing **parametric studies**. It relies on the formalism of **multidimensional grid** introduced hereinafter. It allows to seamlessly **store**, **browse** and **interpolate** results over a parametric space. This tool is a methodological effort and contribution to the field of parametric design and analysis.

## Context

In a **parametric study**, a **system** is analyzed regarding several variables named **parameter**. Each parameter can vary in a discrete  or continuous range of scalar values. If not, the modeling of the system can easily be reinterpreted in that way (for instance a (x,y,z) point parameter can be understood as 3 scalar parameters).

Complex multivariable systems often lead to complex and costly analysis process. In practice, it is almost impossible to know the **state** of the system for every state of inputs ; nor analytically ; nor numerically. It might be a question of pure feasibility but it might also be a question of time or money.

Thus, it is common to analyze the system in a **finite number of interesting states**, where the choice of those states and the corresponding values of the input parameters belong to the knowledge of the designer.

Depending on the sharpness of the grid, the designer would be interested in the results at grid **nodes** or, possibly, to **interpolate** results to predict any state of the system, thus reducing its margin of error.

## Authors

Ivy was first developed by Lionel du Peloux (#THINkSHELL #Navier) for a collaborative research project on shading devices with PhD. Victor Charpentier, Prof. Sigrid Adriaenssens (#FFLab) and Olivier Baverel (#Navier).

## License

Ivy is released under the MITlicense. Anyone is invited to use it and/or contribute through the github page.

## Tool

In practice, `Ivy` is a `.NET` library written in `C#` which comes out as 2 `dlls`:

* `IvyCore.dll` : the core library
* `IvyGh.gha` : an interface for [Grasshopper](http://www.grasshopper3d.com), a parametric design software

![Creation of a 2-Grid with Grasshopper, resulting in 3x3=9 nodes.](doc/gitbook/img/tool.png)

![Grasshopper Toolbar](doc/img/Ivy_gh_toolbar.png)
