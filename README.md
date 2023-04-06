# ImageColourReduction
This is a program which reduce amount of used colours in the given picture.
Colours from RGB color space are the combination of Red, Green and Blue colors, which are represented by eight bits.
It gives 16 777 216 possible colours. 

The program uses Octree method to reduce the amount of colours. There are two versions:
1. in first one - the reduction is after the octree construction
2. the second one - the reduction is while octree is being constructed

# Example
Picture in this example in the beggining uses 36 237 colours. 
When reducted to 8 colours - there is a distinct change, however when reducted to 64 colours - it looks quite similar.

1. Reduction to 8 colours.
![reduction_8col](https://user-images.githubusercontent.com/128033227/230078450-f2511d67-63b4-475e-8f22-dc53ed18c704.png)

1. Reduction to 64 colours.
![image](https://user-images.githubusercontent.com/128033227/230078253-ee4ae194-32b1-479d-94ae-3b46478d82bb.png)

