# ImageColourReduction
This is a program that reduces the number of colors used in a given picture.
Colors from the RGB color space are combinations of red, green, and blue colors, each represented by eight bits, giving a 16,777,216 possible colors.

The program uses the Octree method to reduce the number of colors. There are two versions of the program:
In the first version, the reduction occurs after the Octree is constructed.
In the second version, the reduction occurs while the Octree is being constructed.

# Example
To illustrate the program's capabilities, we can see in the top-left corner the picture taht originally used 36,237 colors.
When picture reducted to 8 colours - there is a distinct change, however when reducted to 64 colours - it looks quite similar.

1. Reduction to 8 colours.
![reduction_8col](https://user-images.githubusercontent.com/128033227/230078450-f2511d67-63b4-475e-8f22-dc53ed18c704.png)

1. Reduction to 64 colours.
![image](https://user-images.githubusercontent.com/128033227/230078253-ee4ae194-32b1-479d-94ae-3b46478d82bb.png)

