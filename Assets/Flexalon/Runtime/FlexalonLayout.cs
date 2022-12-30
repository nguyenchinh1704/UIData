using UnityEngine;

namespace Flexalon
{
    public interface Layout
    {
        // Perform minimal work to determine what the size of node and available size for node's children.
        Bounds Measure(FlexalonNode node, Vector3 size);

        // Position the children of node within the available bounds.
        void Arrange(FlexalonNode node, Vector3 layoutSize);
    }
}