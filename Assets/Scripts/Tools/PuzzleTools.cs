using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Tools
{
    public static class PuzzleTools
    {
        public static IReadOnlyList<Vector2Int> FreeDirections => _freeDirections;
        public static IReadOnlyList<Vector2Int> UpDownDirections => _upDownDirections;
        public static IReadOnlyList<Vector2Int> LeftRightDirections => _leftRightDirections;
        public static IReadOnlyList<Vector2Int> FixedDirections => _fixedDirections;

        private static Vector2Int[] _freeDirections = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
        private static Vector2Int[] _upDownDirections = { new(0, 1), new(0, -1) };
        private static Vector2Int[] _leftRightDirections = { new(1, 0), new(-1, 0) };
        private static Vector2Int[] _fixedDirections = { };


        public static IReadOnlyList<Vector2Int> GetTypePieceMoveDirections(PieceTypeEnum type)
        {
            return type switch
            {
                PieceTypeEnum.Free => FreeDirections,
                PieceTypeEnum.UpDown => UpDownDirections,
                PieceTypeEnum.LeftRight => LeftRightDirections,
                _ => FixedDirections
            };
        }

        public static Sprite[] GetTextureSprite(Texture2D texture2D, in Vector2Int size)
        {
            var sprites = new Sprite[size.x * size.y];
            int cols = size.x; // 横向数量
            int rows = size.y; // 纵向数量

            float pieceWidth = texture2D.width / (float)cols; // 每块宽度
            float pieceHeight = texture2D.height / (float)rows; // 每块高度
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int id = x + y * cols;
                    // 计算小图区域
                    Rect rect = new Rect(
                        x * pieceWidth,
                        (rows - 1 - y) * pieceHeight, // 翻转Y轴，保证图片方向正确
                        pieceWidth,
                        pieceHeight
                    );
                    rect.x = Mathf.Clamp(rect.x, 0, texture2D.width);
                    rect.y = Mathf.Clamp(rect.y, 0, texture2D.height);
                    rect.width = Mathf.Min(rect.width, texture2D.width - rect.x);
                    rect.height = Mathf.Min(rect.height, texture2D.height - rect.y);
                    // 生成Sprite
                    Sprite sprite = Sprite.Create(
                        texture2D,
                        rect,
                        new Vector2(0.5f, 0.5f),
                        1f
                    );
                    sprites[id] = sprite;
                }
            }

            return sprites;
        }

        public static PuzzleModel CutTextureToSprites(
            Texture2D tex,
            int textureId,
            Vector2Int size)
        {
            var puzzleModel = new PuzzleModel(size);
            puzzleModel.SetTexture(tex, textureId);
            int cols = size.x; // 横向数量
            int rows = size.y; // 纵向数量
            var sprites = GetTextureSprite(tex, size);
            var pieces = new PieceModel[cols, rows];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int id = x + y * cols;
                    pieces[x, y] = new PieceModel
                    {
                        Id = id,
                        IsEmpty = id == 0,
                        Type = PieceTypeEnum.Free
                    };
                }
            }

            puzzleModel.SetPuzzle(pieces);
            return puzzleModel;
        }

        /// <summary>
        /// 打乱拼图（保证可解，模拟随机移动空白块）
        /// 根据格子类型判断可移动方向
        /// </summary>
        public static PuzzleModel ShufflePieces(
            this PuzzleModel puzzleModel,
            int shuffleSteps = 150)
        {
            int cols = puzzleModel.Size.x;
            int rows = puzzleModel.Size.y;

            Vector2Int emptyPos = FindEmptyPosition(puzzleModel);
            var valueTuples = new (Vector2Int dir, Vector2Int neighborPos)[4];
            var valueIndex = 0;
            for (int i = 0; i < shuffleSteps; i++)
            {
                // 收集所有可能的移动方向
                valueIndex = 0;

                foreach (var dir in FreeDirections)
                {
                    var neighborPos = emptyPos + dir;

                    // 边界检查
                    if (neighborPos.x < 0 || neighborPos.x >= cols ||
                        neighborPos.y < 0 || neighborPos.y >= rows) continue;

                    var neighborPiece = puzzleModel[neighborPos];

                    // 跳过空白块和固定块
                    if (neighborPiece.IsEmpty || neighborPiece.Type == PieceTypeEnum.Fixed) continue;

                    // 根据邻居格子的类型，判断是否可以向空白块方向移动
                    bool canMove = CanMoveToEmpty(neighborPiece.Type, dir) &&
                                   !puzzleModel.IsBarrier(new(emptyPos, neighborPos));

                    if (canMove)
                    {
                        valueTuples[valueIndex++] = (dir, neighborPos);
                    }
                }

                // 如果没有有效移动，跳过这一步
                if (valueIndex == 0) continue;

                // 随机选择一个有效移动
                var selectedMove = valueTuples[Random.Range(0, valueIndex)];

                // 执行交换：邻居块移动到空白位置
                puzzleModel.Swap(emptyPos, selectedMove.neighborPos);

                // 更新空白块位置
                emptyPos = selectedMove.neighborPos;
            }

            return puzzleModel;

            bool CanMoveToEmpty(PieceTypeEnum type, Vector2Int moveDir)
            {
                return type switch
                {
                    PieceTypeEnum.Free => true, // 自由块可以向任何方向移动
                    PieceTypeEnum.UpDown => moveDir.y != 0, // 只能上下移动
                    PieceTypeEnum.LeftRight => moveDir.x != 0, // 只能左右移动
                    PieceTypeEnum.Fixed => false, // 固定块不能移动
                    _ => false
                };
            }

        }

        /// <summary>
        /// 找到空白块 Id = -1
        /// </summary>
        public static Vector2Int FindEmptyPosition(PuzzleModel puzzleModel)
        {
            int cols = puzzleModel.Size.x;
            int rows = puzzleModel.Size.y;
            for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
                if (puzzleModel[x, y].IsEmpty)
                    return new Vector2Int(x, y);

            return Vector2Int.zero;
        }

        /// <summary>
        /// 随机指定数量的拼图块设为固定（无法移动），直接在pieces数组上操作
        /// </summary>
        public static PuzzleModel RandomPiecesType(this PuzzleModel puzzleModel, int fixedCount = 0,
            int upDownCount = 0, int leftRightCount = 0)
        {
            int cols = puzzleModel.Size.x;
            int rows = puzzleModel.Size.y;

            // 收集所有可用的拼图块（非空白且当前可移动）
            var availableIndices = new List<Vector2Int>();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var piece = puzzleModel[x, y];
                    if (piece is { IsEmpty: false, Type: PieceTypeEnum.Free })
                    {
                        availableIndices.Add(new Vector2Int(x, y));
                    }
                }
            }

            // Fisher-Yates 洗牌，保证随机且不重复
            for (int i = availableIndices.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (availableIndices[i], availableIndices[j]) = (availableIndices[j], availableIndices[i]);
            }

            int count = 0;
            // 设置前fixedCount个为固定块
            for (int i = 0; i < fixedCount && count < availableIndices.Count; i++, count++)
                puzzleModel.SetPieceType(availableIndices[count], PieceTypeEnum.Fixed);
            // 设置前upDownCount个为固定块
            for (int i = 0; i < upDownCount && count < availableIndices.Count; i++, count++)
                puzzleModel.SetPieceType(availableIndices[count], PieceTypeEnum.UpDown);
            // 设置前leftRightCount个为固定块
            for (int i = 0; i < leftRightCount && count < availableIndices.Count; i++, count++)
                puzzleModel.SetPieceType(availableIndices[count], PieceTypeEnum.LeftRight);
            return puzzleModel;
        }

        /// <summary>
        /// 随机创建指定数量的隔板（纯随机，不检查可解性）
        /// </summary>
        public static PuzzleModel RandomBarriers(this PuzzleModel puzzleModel, int barrierCount)
        {
            int cols = puzzleModel.Size.x;
            int rows = puzzleModel.Size.y;

            for (int i = 0; i < barrierCount; i++)
            {
                Vector2Int coord = new Vector2Int(Random.Range(0, cols), Random.Range(0, rows));
                Vector2Int dir = FreeDirections[Random.Range(0, FreeDirections.Count)];
                Vector2Int neighbor = coord + dir;

                if (neighbor.InSize(puzzleModel.Size))
                {
                    puzzleModel.AddBarrier(new(coord, neighbor));
                }
            }

            return puzzleModel;
        }

        public static bool InSize(this Vector2Int coords, in Vector2Int size)
        {
            return coords.x >= 0 && coords.x < size.x && coords.y >= 0 && coords.y < size.y;
        }

        public static int CalculateScore(this PuzzleModel model)
        {
            var step = Mathf.Clamp(Mathf.Floor(model.StepCount - model.Difficulty * 1.5f),
                0,
                model.Difficulty);
            return (int)(100 * ((model.Difficulty - step) / (float)model.Difficulty));
        }
        /// <summary>
        /// 检查某个点的拼图块是否可以交换到相邻位置
        /// </summary>
        /// <returns>如果可以交换，返回交换后的坐标；否则返回 Vector2Int.zero</returns>
        public static Vector2Int? GetValidSwapCoords(this PuzzleModel model, Vector2Int coords)
        {
            if (!coords.InSize(model.Size)) return null;
            var piece = model[coords];
            if (piece.IsEmpty || piece.Type == PieceTypeEnum.Fixed) return null;
            var dir = GetTypePieceMoveDirections(piece.Type);
            for (int i = 0; i < dir.Count; i++)
            {
                var checkCoords = coords + dir[i];
                if (!checkCoords.InSize(model.Size) ||
                    !model[checkCoords].IsEmpty ||
                    model.IsBarrier(new(coords, checkCoords))) continue;
                return checkCoords;
            }
            return null;
        }

        /// <summary>
        /// 检查某个点的拼图块是否可以交换到目标位置
        /// </summary>
        /// <returns>如果可以交换，返回交换后的坐标；否则返回 Vector2Int.zero</returns>
        public static bool IsValidSwapCoords(this PuzzleModel model, Vector2Int coords, Vector2Int targetCoords)
        {
            if (!coords.InSize(model.Size) || !targetCoords.InSize(model.Size)) return false;
            var piece = model[coords];
            var targetPiece = model[targetCoords];
            if (piece.IsEmpty || piece.Type == PieceTypeEnum.Fixed ||
                !targetPiece.IsEmpty ||
                model.IsBarrier(new(coords, targetCoords))) return false;
            if (piece.Type == PieceTypeEnum.Free)
            {
                return true;
            }
            else if (piece.Type == PieceTypeEnum.UpDown)
            {
                return targetCoords.y == coords.y;
            }
            else if (piece.Type == PieceTypeEnum.LeftRight)
            {
                return targetCoords.x == coords.x;
            }
            return false;
        }
        
        public static int CalculateDifficulty(this PuzzleModel model)
        {
            int w = model.Size.x, h = model.Size.y;
            float diff = 0;
            float v = 0;
            // 1. 每个方块到目标位置的曼哈顿距离之和
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    var piece = model[x, y];
                    if (piece.IsEmpty) continue;
            
                    int targetX = piece.Id % w;
                    int targetY = piece.Id / w;
                    diff += Mathf.Abs(x - targetX) + Mathf.Abs(y - targetY);
                }
            }
            // 2. 基础移动量
            diff *= 1.5f;
            // 3. 屏障增益：每有一个屏障 ×1.2
            v += 0.2f * model.Barriers.Count;
            // 4. 特殊类型增益
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    v += model[x, y].Type switch
                    {
                        PieceTypeEnum.Fixed => 0.5f,           // 固定块最难
                        PieceTypeEnum.UpDown => 0.3f,   // 限制移动
                        PieceTypeEnum.LeftRight => 0.3f,
                        _ => 0.0f
                    };
                }
            }
            return Mathf.RoundToInt(diff * (1 + v)) ;
        }
    }
}