using UnityEngine;
using System.Collections.Generic;

namespace Flexalon
{
    public interface FlexalonNode
    {
        GameObject GameObject{ get; }

        void MarkDirty();
        bool Dirty { get; }

        void ForceUpdate();

        FlexalonNode Parent { get; }
        IReadOnlyList<FlexalonNode> Children { get; }
        int Index { get; }
        void AddChild(FlexalonNode child);
        void InsertChild(FlexalonNode child, int index);
        FlexalonNode GetChild(int index);
        void Detach();
        void DetachAllChildren();

        void SetMethod(Layout method);
        Layout Method { get; }

        void SetTransformUpdater(TransformUpdater updater);

        FlexalonObject FlexalonObject { get; }
        void SetFlexalonObject(FlexalonObject obj);

        Vector3 Size { get; }

        Vector3 SizeOfParent { get; }

        Vector3 Offset { get; }

        SizeType GetSizeType(Axis axis);
        SizeType GetSizeType(int axis);

        Vector3 Scale { get; }

        Quaternion Rotation { get; }

        Directions Margin { get; }

        Directions Padding { get; }

        Vector3 GetMeasureSize();

        Vector3 GetArrangeSize();

        Vector3 GetWorldBoxPosition(FlexalonResult result, Vector3 scale, bool includePadding);

        Vector3 GetWorldBoxScale(bool includeLocalScale);

        bool HasResult { get; }
        FlexalonResult Result { get; }

        void SetPositionResult(Vector3 position);
        void SetFillSize(Vector3 size);
        void SetRotationResult(Quaternion quaternion);
        void SetComponentScale(Vector3 scale);

        void SetConstraint(FlexalonConstraint constraint, FlexalonNode target);
        FlexalonConstraint Constraint { get; }

        Adapter Adapter { get; }
        void SetAdapter(Adapter adapter);

        void ApplyScaleAndRotation();
    }
}