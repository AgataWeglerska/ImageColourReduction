using System;
using System.Collections.Generic;
using System.Linq;

namespace Project5
{

    class Octree
    {
        static byte[] Bytes = 
        { 
        0b_0000_0000,
        0b_0000_0001,  // 1
        0b_0000_0010,  // 2
        0b_0000_0100,  // 4
        0b_0000_1000,  // 8
        0b_0001_0000,  // 16
        0b_0010_0000,  // 32
        0b_0100_0000,  // 64
        0b_1000_0000 };  // 128

        public bool isLeaf;
        public Octree[] Children;
        public double R, G, B;
        public int countedPix; // amount of pixels in represented by RGB color or sum of children countedPix
        public int Level; // level is from 0 to 8 - it is the height on which is the node, where root's Level = 0
        public Octree MyParent;
        //--------------------------------------------------------
        // creating octree with RGB values - it is always leaf in the beginning
        public Octree(double R, double G, double B, int CountedPix, int Level, Octree Parent)
        {
            isLeaf = true;
            this.R = R;
            this.G = G;
            this.B = B;
            this.countedPix = CountedPix;
            this.Level = Level;
            this.MyParent = Parent;
        }
        //--------------------------------------------------------
        // method add children to the node, when node has not before any children
        public void AddChildrenWhenNodeIsLeaf(Octree Child)
        {
            Children = new Octree[8];
            isLeaf = false;
            Children[SelectBranch(Child.R, Child.G, Child.B, 9-(this.Level+1))] = Child;
        }
        //--------------------------------------------------------
        // return where to add the node - to which branch - there is 8 branch becasue of R, G, B bites combinations
        static public int SelectBranch(double R, double G, double B, int Level)
        {
            if(((byte)R & Bytes[Level]) == Bytes[0]) //R = 0
            {
                if(((byte)G & Bytes[Level]) == Bytes[0]) // G = 0
                {
                    if (((byte)B & Bytes[Level]) == Bytes[0])// B = 0
                    {
                        return 0;
                    }
                    else // B = 1
                    {
                        return 1;
                    }
                }
                else // G = 1
                {
                    if (((byte)B & Bytes[Level]) == Bytes[0])// B = 0
                    {
                        return 2;
                    }
                    else // B = 1
                    {
                        return 3;
                    }
                }
            }
            else // R = 1
            {
                if (((byte)G & Bytes[Level]) == Bytes[0]) // G = 0
                {
                    if (((byte)B & Bytes[Level]) == Bytes[0])// B = 0
                    {
                        return 4;
                    }
                    else // B = 1
                    {
                        return 5;
                    }
                }
                else // G = 1
                {
                    if (((byte)B & Bytes[Level]) == Bytes[0])// B = 0
                    {
                        return 6;
                    }
                    else // B = 1
                    {
                        return 7;
                    }
                }
            }
        }
        // ------------------------------------------------------------
        // below functions are used to make copy of octree but in this program there is no need to do so
        public Octree MakeCopyOfOneNode(Octree NodeParent)
        {
            return new Octree(this.R, this.G, this.B, this.countedPix,this.Level, NodeParent);
        }
        public Octree MakeCopyOfWholeTree()
        {
            return CopyRecurential(this, null);
        }
        private Octree CopyRecurential(Octree Tree, Octree _Parent)
        {
            if (Tree.isLeaf == true)
            {
                return Tree.MakeCopyOfOneNode(_Parent);
            }
            else
            {
                Octree NewTree = Tree.MakeCopyOfOneNode(_Parent);
                NewTree.isLeaf = false;
                NewTree.Children = new Octree[8];
                for(int i = 0; i < 8; i++)
                {
                    if (Tree.Children[i] != null)
                    {
                        NewTree.Children[i] = CopyRecurential(Tree.Children[i], NewTree);
                    }
                }
                return NewTree;
            }
        }
    }
}
