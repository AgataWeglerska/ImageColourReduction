using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Project5
{
    class CreateOctreeSecondMethod
    {
        Octree Root;
        Bitmap Picture;
        LockBitMap locked;
        int MaxColors;

        public CreateOctreeSecondMethod(Bitmap Picture, int MaxCol, ProgressBar prBar)
        {
            // at first Root is created
            int Level = 0; // Root level
            int countedPixel = 1; // Root countedPixel
            this.Picture = Picture;
            Color Col = Picture.GetPixel(0, 0);
            this.Root = new Octree(Col.R,Col.G, Col.B, countedPixel, Level, null);

            this.MaxColors = MaxCol;
            locked = new LockBitMap(Picture);
            BuildTree(prBar);
        }
        // -------------------------------------------------------
        // main function starting to build the Octree
        public void BuildTree(ProgressBar prBar)
        {
            int bits = (Picture.Height * Picture.Width)/16;
            List<Octree>[] ParentsList = new List<Octree>[8];
            int LeafCounter = 1;
            locked.LockBits();
            // Root has been created
            int i = 0;
            for (int j = 1; j < Picture.Height; j++)
            {
                if (InsertTree(Root, locked.GetPixel(i, j), ParentsList))
                {
                    LeafCounter++;
                    while (LeafCounter > MaxColors)
                    {
                        int temp = Reduction(ParentsList);
                        LeafCounter -= temp;
                    }
                }
            }

            for (i = 1; i < Picture.Width; i++)
            {
                for (int j = 0; j < Picture.Height; j++)
                {
                    // refreshing progressbar
                    if (((i+1) * Picture.Height + (j+1)) % bits == 0) 
                    {
                        if (prBar.Value != prBar.Maximum)
                            prBar.Value += 1;
                        prBar.Refresh();
                    }

                    if (InsertTree(Root, locked.GetPixel(i, j), ParentsList))
                    {
                        LeafCounter++;
                        while (LeafCounter > MaxColors)
                        {
                            LeafCounter -= Reduction(ParentsList);
                        }
                    }
                }
            }
            locked.UnlockBits();
            return;
        }
        // inserting Octree nodes - recursion
        private bool InsertTree(Octree Tree, Color RGB, List<Octree>[] ParentsLists)
        {
            Tree.countedPix++; // Adding one pixel to every node
            Octree ChildNew, ChildOld, Next;
            if (Tree.isLeaf == true)
            {
                if (RGB.R != (int)Tree.R || RGB.G != (int)Tree.G || RGB.B != (int)Tree.B)
                {
                    ChildOld = new Octree(Tree.R, Tree.G, Tree.B, Tree.countedPix - 1, Tree.Level + 1, Tree);
                    Tree.AddChildrenWhenNodeIsLeaf(ChildOld);
                    return InsertTree(Tree, RGB, ParentsLists);
                }
                else
                {
                    // updating the colour in Tree because R,G and B are double types
                    // - to accomplish more precise colors after redustion
                    Tree.R = ((Tree.countedPix - 1) * Tree.R + RGB.R) / Tree.countedPix;
                    Tree.G = ((Tree.countedPix - 1) * Tree.G + RGB.G) / Tree.countedPix;
                    Tree.B = ((Tree.countedPix - 1) * Tree.B + RGB.B) / Tree.countedPix;
                    return false; // when there is no new colors
                }
            }
            else
            {
                int position = Octree.SelectBranch(RGB.R, RGB.G, RGB.B, 9 - (Tree.Level + 1));
                Next = Tree.Children[position];
                if (Next != null)
                    return InsertTree(Next, RGB, ParentsLists);
                else
                {
                    ChildNew = new Octree(RGB.R, RGB.G, RGB.B, 1, Tree.Level + 1, Tree); // node with this rgb never has existed before => countedPixels == 1
                    Tree.Children[position] = ChildNew;
                    InsertParent(Tree, ParentsLists);
                }
            }
            return true;
        }
        // inserting parent to the apprioprate list
        private void InsertParent(Octree Parent, List<Octree>[] ParentsLists)
        {
            int Level = Parent.Level;
            if (ParentsLists[Level] == null)
            {
                ParentsLists[Level] = new List<Octree>();
                ParentsLists[Level].Add(Parent);
            }
            else
            {
                if (!ParentsLists[Level].Contains(Parent))
                {
                    ParentsLists[Level].Add(Parent);
                }
            }
        }
        // function starting reduction of colours
        private int Reduction(List<Octree>[] ParentsLists) // returns how many children it removed minus one
        {
            int index = -1;
            int ChildrenCounter;
            // finding list with the highest index, wchih is not null
            for (int i = 7; i >= 0; i--)
            {
                if (ParentsLists[i] != null && ParentsLists[i].Count != 0)
                {
                    index = i;
                    break;
                }
            }
            Octree ParentToReduce = ParentWithMinCountedPix(ParentsLists[index]);
            ChildrenCounter = ReduceChildren(ParentToReduce);
            RemoveAndAdd(ParentToReduce, ParentsLists);
            return ChildrenCounter;
        }
        // finding parent with the minimal number of counted pixels
        private Octree ParentWithMinCountedPix(List<Octree> ParentsList)
        {
            Octree Tree = ParentsList[0];
            foreach(var el in ParentsList)
            {
                if (el.countedPix < Tree.countedPix)
                    Tree = el;
            }
            return Tree;

        }
        // deleting children
        private int ReduceChildren(Octree Parent)
        {
            int childcount = 0;
            int scale = 0;
            double SumR = 0, SumG = 0, SumB = 0;
            for (int i = 0; i < 8; i++)
            {
                if (Parent.Children[i] != null)
                {
                    childcount++;
                    scale += Parent.Children[i].countedPix;
                    SumR += Parent.Children[i].countedPix * Parent.Children[i].R;
                    SumG += Parent.Children[i].countedPix * Parent.Children[i].G;
                    SumB += Parent.Children[i].countedPix * Parent.Children[i].B;
                }
                Parent.Children[i] = null;
            }
            Parent.isLeaf = true;
            SumR /= scale;
            SumG /= scale;
            SumB /= scale;
            Parent.R = SumR;
            Parent.G = SumG;
            Parent.B = SumB;
            return childcount - 1;
        }
        // updateing list of parents - nodes with children
        private void RemoveAndAdd(Octree ParentToDelete, List<Octree>[] ParentsLists)
        {
            if (ParentToDelete.Level != 0)
                InsertParent(ParentToDelete.MyParent, ParentsLists);
            ParentsLists[ParentToDelete.Level].Remove(ParentToDelete);
        }
        // -------------------------------------------------------
        // finding picture with reduced colors
        public void ReducePicture(ref Bitmap ViewedPicture)
        {
            LockBitMap locked2 = new LockBitMap(ViewedPicture);
            locked2.LockBits();
            locked.LockBits();
            Color PixelColor, ReducedColor;
            for (int x = 0; x < ViewedPicture.Width; x++)
            {
                for (int y = 0; y < ViewedPicture.Height; y++)
                {
                    PixelColor = locked.GetPixel(x, y);
                    ReducedColor = FindColor(PixelColor);
                    locked2.SetPixel(x, y, ReducedColor);
                }
            }
            locked2.UnlockBits();
            locked.UnlockBits();
        }
        // finding color in the octree
        private Color FindColor(Color PixelColor)
        {
            int Branch;
            Octree Temp = Root;
            int Level = 1;
            while (Temp.isLeaf == false)
            {
                Branch = Octree.SelectBranch(PixelColor.R, PixelColor.G, PixelColor.B, 9 - Level);
                Level++;
                if (Temp.Children[Branch] == null)
                {
                    break;
                }
                Temp = Temp.Children[Branch];
            }
            return Color.FromArgb((int)Temp.R, (int)Temp.G, (int)Temp.B);
        }
    }
}
