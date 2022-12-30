using UnityEngine;

namespace Flexalon
{
    [ExecuteAlways, AddComponentMenu("Flexalon/Grid Layout"), HelpURL("https://www.flexalon.com/docs/gridLayout")]
    public class FlexalonGridLayout : LayoutBase
    {
        public enum CellTypes
        {
            Rectangle,
            Hexagonal
        }

        [SerializeField]
        private CellTypes _cellType = CellTypes.Rectangle;
        public CellTypes CellType
        {
            get { return _cellType; }
            set { _cellType = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private uint _columns = 3;
        public uint Columns
        {
            get { return _columns; }
            set { _columns = System.Math.Max(value, 1); _node.MarkDirty(); }
        }

        [SerializeField]
        private uint _rows = 3;
        public uint Rows
        {
            get { return _rows; }
            set { _rows = System.Math.Max(value, 1); _node.MarkDirty(); }
        }

        [SerializeField]
        private Direction _columnDirection = Direction.PositiveX;
        public Direction ColumnDirection
        {
            get { return _columnDirection; }
            set { _columnDirection = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Direction _rowDirection = Direction.NegativeY;
        public Direction RowDirection
        {
            get { return _rowDirection; }
            set { _rowDirection = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _columnSpacing = 0;
        public float ColumnSpacing
        {
            get { return _columnSpacing; }
            set { _columnSpacing = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _rowSpacing = 0;
        public float RowSpacing
        {
            get { return _rowSpacing; }
            set { _rowSpacing = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Align _horizontalAlign = Align.Center;
        public Align HorizontalAlign
        {
            get { return _horizontalAlign; }
            set { _horizontalAlign = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Align _verticalAlign = Align.Center;
        public Align VerticalAlign
        {
            get { return _verticalAlign; }
            set { _verticalAlign = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Align _depthAlign = Align.Center;
        public Align DepthAlign
        {
            get { return _depthAlign; }
            set { _depthAlign = value; _node.MarkDirty(); }
        }

        public override Bounds Measure(FlexalonNode node, Vector3 size)
        {
            FlexalonLog.Log("GridMeasure | Size", node, size);

            var rowAxis = (int) Math.GetAxisFromDirection(_rowDirection);
            var columnAxis = (int) Math.GetAxisFromDirection(_columnDirection);
            var thirdAxis = Math.GetThirdAxis(rowAxis, columnAxis);
            var cellSize = size;
            cellSize[rowAxis] = GetRowSize(size[rowAxis]);
            cellSize[columnAxis] = GetColumnSize(size[columnAxis]);

            foreach (var child in node.Children)
            {
                var childSize = child.GetMeasureSize();
                if (node.GetSizeType(rowAxis) == SizeType.Layout)
                {
                    cellSize[rowAxis] = Mathf.Max(childSize[rowAxis], cellSize[rowAxis]);
                }

                if (node.GetSizeType(columnAxis) == SizeType.Layout)
                {
                    cellSize[columnAxis] = Mathf.Max(childSize[columnAxis], cellSize[columnAxis]);
                }

                if (node.GetSizeType(thirdAxis) == SizeType.Layout)
                {
                    cellSize[thirdAxis] = Mathf.Max(cellSize[thirdAxis], childSize[thirdAxis]);
                }
            }

            base.Measure(node, cellSize);

            size[rowAxis] = cellSize[rowAxis] * _rows + (_rowSpacing * (_rows - 1));
            size[columnAxis] = cellSize[columnAxis] * _columns + (_columnSpacing * (_columns - 1));
            size[thirdAxis] = cellSize[thirdAxis];
            return new Bounds(Vector3.zero, size);
        }

        private float GetRowSize(float availableRowSize)
        {
            if (_cellType == CellTypes.Rectangle)
            {
                return (availableRowSize - _rowSpacing * (_rows - 1)) / _rows;
            }
            else
            {
                return (availableRowSize - _rowSpacing * (_rows - 1)) / (1 + (_rows - 1) * 0.75f);
            }
        }

        private float GetColumnSize(float availableColumnSize)
        {
            if (_cellType == CellTypes.Rectangle)
            {
                return (availableColumnSize - _columnSpacing * (_columns - 1)) / _columns;
            }
            else
            {
                var sz = (availableColumnSize - _columnSpacing * (_columns - 1)) / _columns;
                if (_rows > 1)
                {
                    sz *= _columns / (_columns + 0.5f);
                }

                return sz;
            }
        }

        private Vector3 GetPosition(uint row, uint column, int rowAxis, int columnAxis, float rowSize, float columnSize, Vector3 size)
        {
            var position = new Vector3();
            var startRow = -size[rowAxis] / 2;
            var startCol = -size[columnAxis] / 2;

            if (_cellType == CellTypes.Rectangle)
            {
                position[rowAxis] = startRow + rowSize * row + _rowSpacing * row + rowSize / 2;
                position[columnAxis] = startCol + columnSize * column + _columnSpacing * column + columnSize / 2;
            }
            else
            {
                bool rowEven = (row % 2) == 0;
                position[rowAxis] = startRow + rowSize * 0.75f * row + _rowSpacing * row + rowSize / 2;
                position[columnAxis] = startCol + columnSize * column + columnSize / 2 + _columnSpacing * column + (rowEven ? 0 : columnSize / 2);
            }

            position[rowAxis] *= Math.GetPositiveFromDirection(_rowDirection);
            position[columnAxis] *= Math.GetPositiveFromDirection(_columnDirection);

            return position;
        }

        private void PositionChild(FlexalonNode child, uint row, uint column, int rowAxis, int columnAxis, Vector3 cellSize, Vector3 size)
        {
            var position = GetPosition(row, column, rowAxis, columnAxis, cellSize[rowAxis], cellSize[columnAxis], size);
            var aligned = Math.Align(child.GetArrangeSize(), cellSize, _horizontalAlign, _verticalAlign, _depthAlign);
            child.SetPositionResult(position + aligned);
        }

        public override void Arrange(FlexalonNode node, Vector3 layoutSize)
        {
            FlexalonLog.Log("GridArrange | LayoutSize", node, layoutSize);

            var rowAxis = (int) Math.GetAxisFromDirection(_rowDirection);
            var columnAxis = (int) Math.GetAxisFromDirection(_columnDirection);
            float rowSize = GetRowSize(layoutSize[rowAxis]);
            float columnSize = GetColumnSize(layoutSize[columnAxis]);

            uint nextR = 0;
            uint nextC = 0;

            var cellSize = layoutSize;
            cellSize[rowAxis] = rowSize;
            cellSize[columnAxis] = columnSize;

            // Position remaining childing in column-row order.
            foreach (var child in node.Children)
            {
                PositionChild(child, nextR, nextC, rowAxis, columnAxis, cellSize, layoutSize);

                nextC += 1;
                if (nextC >= _columns)
                {
                    nextR = nextR + 1;
                    nextC = 0;
                }
            }
        }

        void OnValidate()
        {
            _rows = System.Math.Max(_rows, 1u);
            _columns = System.Math.Max(_columns, 1u);
        }

        void OnDrawGizmosSelected()
        {
            if (_node != null)
            {
                var rowAxis = (int) Math.GetAxisFromDirection(_rowDirection);
                var columnAxis = (int) Math.GetAxisFromDirection(_columnDirection);
                var sz = _node.Result.AdapterBounds.size - _node.Padding.Size;
                float rowSize = GetRowSize(sz[rowAxis]);
                float columnSize = GetColumnSize(sz[columnAxis]);

                Gizmos.color = new Color(1, 1, 0, 0.5f);
                var scale = _node.GetWorldBoxScale(true);
                Gizmos.matrix = Matrix4x4.TRS(_node.GetWorldBoxPosition(_node.Result, scale, true), transform.rotation, scale);

                for (uint r = 0; r < _rows; r++)
                {
                    for (uint c = 0; c < _columns; c++)
                    {
                        var position = GetPosition(r, c, rowAxis, columnAxis, rowSize, columnSize, sz);
                        if (_cellType == CellTypes.Rectangle)
                        {
                            DrawRectangle(position, rowSize, columnSize, rowAxis, columnAxis);
                        }
                        else
                        {
                            DrawHexagon(position, rowSize, columnSize, rowAxis, columnAxis);
                        }
                    }
                }
            }
        }

        void DrawRectangle(Vector3 position, float rowSize, float columnSize, int rowAxis, int columnAxis)
        {
            var p1 = new Vector3(); // top right
            p1[rowAxis] = rowSize / 2;
            p1[columnAxis] = columnSize / 2;

            var p2 = new Vector3(); // bottom right
            p2[rowAxis] = -rowSize / 2;
            p2[columnAxis] = columnSize / 2;

            var p3 = new Vector3(); // bottom left
            p3[rowAxis] = -rowSize / 2;
            p3[columnAxis] = -columnSize / 2;

            var p4 = new Vector3(); // top left
            p4[rowAxis] = rowSize / 2;
            p4[columnAxis] = -columnSize / 2;

            Gizmos.DrawLine(position + p1, position + p2);
            Gizmos.DrawLine(position + p2, position + p3);
            Gizmos.DrawLine(position + p3, position + p4);
            Gizmos.DrawLine(position + p4, position + p1);
        }

        void DrawHexagon(Vector3 position, float rowSize, float columnSize, int rowAxis, int columnAxis)
        {
            var p1 = new Vector3(); // top
            p1[rowAxis] = rowSize / 2;

            var p2 = new Vector3(); // top right
            p2[rowAxis] = rowSize / 4;
            p2[columnAxis] = columnSize / 2;

            var p3 = new Vector3(); // bottom right
            p3[rowAxis] = -rowSize / 4;
            p3[columnAxis] = columnSize / 2;

            var p4 = new Vector3(); // bottom
            p4[rowAxis] = -rowSize / 2;

            var p5 = new Vector3(); // bottom left
            p5[rowAxis] = -rowSize / 4;
            p5[columnAxis] = -columnSize / 2;

            var p6 = new Vector3(); // top left
            p6[rowAxis] = rowSize / 4;
            p6[columnAxis] = -columnSize / 2;

            Gizmos.DrawLine(position + p1, position + p2);
            Gizmos.DrawLine(position + p2, position + p3);
            Gizmos.DrawLine(position + p3, position + p4);
            Gizmos.DrawLine(position + p4, position + p5);
            Gizmos.DrawLine(position + p5, position + p6);
            Gizmos.DrawLine(position + p6, position + p1);
        }
    }
}