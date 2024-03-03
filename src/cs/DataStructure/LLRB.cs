namespace DataStructure;

public class LLRBTree<TKey, TValue> where TKey : IComparable<TKey>
{
    public class LLRBTreeNode<TKey, TValue> where TKey : IComparable<TKey>
    {
        public TKey Key { get; private set; }
        public TValue Value { get; private set; }
        public LLRBTreeNode<T> Left { get; set; }
        public LLRBTreeNode<T> Right { get; set; }
        public bool IsBlack { get; set; }

        public bool IsRed => !IsBlack;

        public LLRBTreeNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            IsBlack = false;
        }
    }
    private LLRBTreeNode<TKey, TValue> _root;
    private int _count;

    public LLRBTree()
    {}

    public LLRBTree(IEnumerable<KeyValuePair<TKey, TValue>> kvps)
    {
        foreach (var kvp in kvps)
        {
            Add(kvp.Key, kvp.Value);
        }
    }

    public void Add(TKey key, TValue value)
    {
        throw new NotImplementedException();
    }

    public void Delete(TKey key)
    {
        throw new NotImplementedException();
    }

    public TValue Get(TKey key)
    {
        if (TryGetValue(_root, key, out var value))
        {
            return value;
        }

        throw new KeyNotFoundException();
    }

    public bool TryGetValue(TKey key, out TValue value)
        => TryGetValue(_root, key, out value);

    public void Insert(TKey key, TValue value)
    {
        _root = Insert(_root, key, value);
        _root.IsBlack = true;
    }

    public bool TryInsert(TKey key, TValue value)
    {
        try
        {
            _root = Insert(_root, key, value);
            _root.IsBlack = true;
        }
        catch (ApplicationException ex)
        {
            if (ex.Message == "Key already exists")
            {
                return false;
            }

            throw;
        }

        return true;
    }

    public bool Delete(TKey key)
    {
        throw new NotImplementedException();
    }

    private LLRBTreeNode<TKey, TValue> Insert(LLRBTreeNode<TKey, TValue> root, TKey key, TValue value)
    {
        if (root == null)
        {
            _count++;
            return new LLRBTreeNode<TKey, TValue>(key, value);
        }

        var comp = root.Key.CompareTo(key);
        if (comp == 0)
        {
            throw new ApplicationException("Key already exists");
        }

        if (comp > 0)
        {
            root.Left = Insert(root.Left, key, value);
        }
        else
        {
            root.Right = Insert(root.Right, key, value);
        }

        // fix root
        /// right lean is not allowed
        /// after the rotate left, root.Left cannot be Black anymore
        if (root.Left.IsBlack && root.Right.IsRed)
        {
            root = RotateLeft(root);
        }

        /// cannot have two left lean in two near hierarchy
        if (root.Left.IsRed && root.Left?.Left?.IsRed == true)
        {
            root = RotateRight(root);
        }

        /// 4 node is not allowed
        if (root.Left.IsRed && root.Right.IsRed)
        {
            FlipColor(root);
        }

        return root;
    }

    private bool TryGetValue(LLRBTreeNode<TKey, TValue> root, TKey key, out TValue value)
    {
        if (root == null)
        {
            value = default(TValue);
            return false;
        }

        var comp = root.Key.CompareTo(key);
        if (comp == 0)
        {
            value = root.Value;
            return true;
        }

        return TryGetValue(comp > 0 ? root.Left : root.Right, key, out value);
    }

    /// <summary>
    ///  2              3
    /// 1 3      =>    2
    ///               1
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    private LLRBTreeNode<TKey, TValue> RotateLeft(LLRBTreeNode<TKey, TValue> root)
    {
        var left = root.Left;
        var right = root.Right;
        root.Right = right.Left;
        right.Left = root;
        (root.IsBlack, right.IsBlack) = (right.IsBlack, root.IsBlack);
        return right;
    }

    /// <summary>
    ///  2              1
    /// 1 3     =>       2
    ///                   3
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    private LLRBTreeNode<TKey, TValue> RotateRight(LLRBTreeNode<TKey, TValue> root)
    {
        var left = root.Left;
        var right = root.Right;
        root.Left = left.Right;
        left.Right = root;
        (root.IsBlack, left.IsBlack) = (left.IsBlack, root.IsBlack);
        return left;
    }

    /// <summary>
    /// Flip color, to make 4 node to 2 node and propagate to parent
    /// </summary>
    private void FlipColor(LLRBTreeNode<TKey, TValue> root)
    {
        root.Left.IsBlack = true;
        root.Right.IsBlack = true;
        root.IsBlack = false;
    }
}