# Unity-Humanoid-AdjustProportions
Adjust a character's body proportions without setting up blendshapes

# Background
* Most character creation tools allow to adjust many details of a character's proportions. To dynamically change a character 
on-the-run inside Unity, one typically sets up blendshapes which can be interpreted by Unity. 
* This script takes a faster route by directly ajusting the character's bones and mesh vertices without setting up blendshapes.
This allows for several alterations of a character. For more refined or artistic real-time character deformation, blendshapes
are however still recommended. 

# Use
* Simply attach the script "MeshDeformation.cs" to your rigged, humanoid character and press start. This has been tested with
characters made with Autodesk Character Generator. For characters made with other systems, the script may need adjustments. 
* The script works well with Inverse Kinematics systems that operate in the LateUpdate()-Loop (like e.g. Final IK), but does not
work with Unity's native IK system. 
![alt tag](https://github.com/mariusrubo/Unity-Humanoid-AdjustProportions/blob/master/UnityMeshDeform.gif)
