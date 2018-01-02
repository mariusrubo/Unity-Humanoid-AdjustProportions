# Unity-Humanoid-AdjustProportions
Adjust a character's body proportions without setting up blendshapes

# Background
* Most character creation tools allow to adjust many details of a character's proportions. To dynamically change a character 
on-the-run inside Unity, however, one typically needs to set up blendshapes which can be interpreted by Unity. 
* This script takes a faster route by directly ajusting the character's bones and mesh vertices without using blendshapes.
As the Gif below shows, this allows for several alterations of a character, but does not reach the refinement of carefully crafted blendshapes. 

# Use
* Attach the script "MeshDeformation.cs" to your rigged, humanoid character standing in a T-Pose. Drag its SkinnedMeshRenderer component to the corresponding field and press start. This has been tested with characters made with Autodesk Character Generator. For characters made with other systems (i.e., if they have a different count or setup of bones), the script may need adjustments. 
* This approach works in parallel with Inverse Kinematics systems that operate in the LateUpdate()-Loop (like e.g. Final IK). To use it with Unity's native IK system or with animations, one has to first perform adjustments to the character's proportions, then switch off the script MeshDeformation.cs, and only then swith on IK or animation. The reason for this is that "MeshDeformation.cs" assumes the character to be in a T-Pose for as long as the Update()-loop is running. It produces bizarre deformations when this condition is not met.  

![alt tag](https://github.com/mariusrubo/Unity-Humanoid-AdjustProportions/blob/master/UnityMeshDeform.gif)
