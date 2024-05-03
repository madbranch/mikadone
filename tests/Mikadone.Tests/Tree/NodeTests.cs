using FluentAssertions;

namespace Mikadone.Tree;

public abstract class NodeTests<TNode>
   where TNode : INode<TNode>, new()
{
  [Fact]
  public void Constructor_Default_ShouldBeEmpty()
  {
    TNode node = new();

    node.Count
      .Should()
      .Be(0);
  }

  [Fact]
  public void Constructor_Default_ParentShouldBeNull()
  {
    TNode node = new();
    
    node.Parent.Should().BeNull();
  }

  [Fact]
  public void Add_SomeValideNode_ParentShouldBeSet()
  {
    TNode root = new();

    TNode child = new();

    root.Add(child);

    child.Parent.Should().BeSameAs(root);
  }
}