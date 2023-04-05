using System.Drawing;
using System.Windows.Forms;

namespace Project5
{
    // MyList is used to add parents of the leafs to know wchih nodes to reduce at first
    class MyList 
    {
        public Octree Parent;
        public MyList Next;

        public MyList(Octree parent, MyList next)
        {
            this.Parent = parent;
            this.Next = next;
        }
    }

    class CreateOctree
    {
        Octree Root;
        int MaxColors;
        Bitmap Picture;
        LockBitMap locked;

        public CreateOctree(Bitmap Picture, int MaxCol, ProgressBar prBar)
        {
            // at first Root is created
            int Level = 0; // Root Level
            int countedPixel = 1; // Root countedPixel
            this.Picture = Picture;
            Color Col = Picture.GetPixel(0, 0);
            this.Root = new Octree(Col.R, Col.G, Col.B,countedPixel,Level,null);
            this.MaxColors = MaxCol;

            locked = new LockBitMap(Picture);
            prBar.Value = 10;
            AddAllPixels();
            prBar.Value+=10;
            prBar.Refresh();
            ReduceToMax(prBar);
        }
        // -------------------------------------------------------
        // Adding every pixel to the tree
        private void AddAllPixels()
        {
            // Root has been created in the constructor
            locked.LockBits();           
            int i = 0;
            for(int j = 1; j < Picture.Height; j++)
            {
                InsertTree(Root, locked.GetPixel(i,j));
            }

            for(i = 1; i < Picture.Width; i++)
            {
                for (int j = 0; j < Picture.Height; j++)
                {
                    InsertTree(Root, locked.GetPixel(i, j));
                }
            }
            locked.UnlockBits();
            return;
        }
        // Inserting Tree nodes - recursion
        private void InsertTree(Octree Tree, Color RGB)
        {
            Octree ChildNew, ChildOld, Next;
            if (Tree.isLeaf == true)
            {
               if (RGB.R != (int)Tree.R || RGB.G != (int)Tree.G || RGB.B != (int)Tree.B)
                {
                    ChildOld = new Octree(Tree.R, Tree.G, Tree.B,Tree.countedPix,Tree.Level+1,Tree);
                    Tree.AddChildrenWhenNodeIsLeaf(ChildOld); 
                    InsertTree(Tree, RGB);
                }
                else
                {
                    // updating the colour in Tree because R,G and B are double types
                    // - to accomplish more precise colors after redustion
                    Tree.R = ((Tree.countedPix) * Tree.R + RGB.R) / (Tree.countedPix+1);
                    Tree.G = ((Tree.countedPix) * Tree.G + RGB.G) / (Tree.countedPix + 1);
                    Tree.B = ((Tree.countedPix) * Tree.B + RGB.B) / (Tree.countedPix + 1);
                }
            }
            else
            {
                int position = Octree.SelectBranch(RGB.R, RGB.G, RGB.B, 9 - (Tree.Level + 1));
                Next = Tree.Children[position];
                if(Next != null)
                    InsertTree(Next, RGB);
                else
                {
                    ChildNew = new Octree(RGB.R, RGB.G, RGB.B, 1, Tree.Level + 1,Tree); // node with this rgb never has existed before => countedPixels == 1
                    Tree.Children[position] = ChildNew;
                }
            }
            Tree.countedPix++; // Adding one pixel to every node
        }
        // -------------------------------------------------------
        // reducing amount of colors to MaxColors
        public void ReduceToMax(ProgressBar prBar)
        {
            int Level = 7; // Level on witch parent is
            int SumOfCol = 0;
            MyList[] ParentsList = new MyList[8];
            for(int i = 0; i < 8; i++)
            {
                if(Root.Children[i] != null)
                    FindParents(Root.Children[i] ,ParentsList, ref SumOfCol);
            }

            prBar.Value += 10;
            prBar.Refresh();

            int LoadBar = (SumOfCol-MaxColors) / 10;
            int count = 9;
            while (SumOfCol > MaxColors) // MaxColors value must be bigger then 0
            {
                if((SumOfCol/ LoadBar) == count)
                {
                    count--;
                    if(prBar.Value != prBar.Maximum)
                        prBar.Value += 10;
                    prBar.Refresh();
                }

                if (ParentsList[Level] == null)
                    Level--;
                else
                {
                    SumOfCol -= ReduceChildren(ParentsList[Level].Parent);
                    RemoveAndAdd(Level, ParentsList);
                }
            }
            return;
        }
        // finding parents of every leaf - on different levels and adding them to MyLists
        private void FindParents(Octree Tree, MyList[] ParentsLitst, ref int SumOfCol) {
            if(Tree.isLeaf == true)
            {
                SumOfCol++;
                if (Tree.MyParent.Level == 0)
                    return;
                InsertParent(Tree.MyParent,ParentsLitst);
            }
            else
            {
                foreach(var CH in Tree.Children)
                {
                    if (CH != null)
                        FindParents(CH, ParentsLitst, ref SumOfCol);
                }
            }
        }
        // inserting parent to the apprioprate MyList - on the appriopriate position
        // - nodes are sorted by counted pixels
        private void InsertParent(Octree Parent, MyList[] ParentsLists)
        {
            int Level = Parent.Level;
            if (ParentsLists[Level] == null)
                ParentsLists[Level] = new MyList(Parent, null);
            else
            {
                MyList Temp = ParentsLists[Level];

                if (Temp.Parent == Parent)
                    return;
                if (Temp.Parent.countedPix > Parent.countedPix)
                {
                    MyList Pom = new MyList(Parent, Temp);
                    ParentsLists[Level] = Pom;
                }
                else if (Temp.Next == null)
                {
                    Temp.Next = new MyList(Parent, null);
                }
                else
                {
                    while (Temp.Next != null && Temp.Next.Parent.countedPix < Parent.countedPix)
                        Temp = Temp.Next;
                    if (Temp.Next == null)
                        Temp.Next = new MyList(Parent, null);
                    else
                    {
                        if (Temp.Next.Parent.countedPix > Parent.countedPix)
                        {
                            MyList Pom = new MyList(Parent, Temp.Next);
                            Temp.Next = Pom;
                        }
                        else
                        {
                            int count = Parent.countedPix;
                            while (Temp.Next != null && Temp.Next.Parent.countedPix == count)
                            {
                                if (Parent == Temp.Next.Parent)
                                    return;
                                Temp = Temp.Next;
                            }
                            MyList Pom = new MyList(Parent, Temp.Next);
                            Temp.Next = Pom;
                        }
                    }
                }
            }
        }
        // deleting children
        private int ReduceChildren(Octree Parent)
        {
            int childcount=0;
            int scale = 0;
            double SumR = 0, SumG = 0, SumB = 0;
            for(int i =0;i<8;i++)
            {
                if(Parent.Children[i] != null)
                {
                    childcount++;
                    scale+=Parent.Children[i].countedPix;
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
            return childcount-1;
        }
        // updateing MyLists of parents - nodes witch children
        private void RemoveAndAdd(int Level, MyList[] ParentsLists)
        {
            MyList Temp = ParentsLists[Level];
            ParentsLists[Level] = ParentsLists[Level].Next;
            if (Temp.Parent.Level == 0)
                return;
            InsertParent(Temp.Parent.MyParent, ParentsLists);
        }
        // -------------------------------------------------------
        // New Bitmap - if I would like to use the Octree more times it could be slightly faster
        /*
        public void CopyBit()
        {
            Root = CopiedRoot.MakeCopyOfWholeTree();
            return;
        }
        */
        // -------------------------------------------------------
        // finding picture with reduced colors
        public void ReducePicture(ref Bitmap ViewedPicture)
        {
            LockBitMap locked2 = new LockBitMap(ViewedPicture);
                locked2.LockBits();
                locked.LockBits();
            Color PixelColor, ReducedColor;
            for(int x = 0; x < ViewedPicture.Width; x++)
            {
                for (int y = 0;y< ViewedPicture.Height; y++)
                {
                    PixelColor = locked.GetPixel(x,y);
                    ReducedColor = FindColor(PixelColor);
                    locked2.SetPixel(x,y,ReducedColor);
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
            while(Temp.isLeaf == false)
            {
                Branch = Octree.SelectBranch(PixelColor.R, PixelColor.G, PixelColor.B, 9-Level);
                Level++;
                Temp = Temp.Children[Branch];
            }
            return Color.FromArgb((int)Temp.R,(int)Temp.G, (int)Temp.B);
        }
    }
}
