public class Node<T>
{
    public T Value { get; set; }

    public Node<T>? Parent { get; set; }

    public List<Node<T>> Children { get; } = [];

    public Node(T value)
    {
        Value = value;
    }

    public Node(T value, Node<T> parent)
    {
        Value = value;
        Parent = parent;
        parent.Children.Add(this);
    }
}