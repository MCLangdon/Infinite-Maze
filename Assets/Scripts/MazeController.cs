using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MazeController : MonoBehaviour {

    public Transform maze;
    public Transform mazeRowFloor;
    public Transform nsMazeWall;
    public Transform ewMazeWall;
    public Collider mazeRowEntrance;
    public Collider projectile;
    public Transform nsMazeBoundary;
    public Transform ceilingMazeBoundary;
    public Transform ewMazeBoundary;

    private int numOfRows;
    private int numOfSets;
    private bool mazeCompleted;

    private static List<MazeCell[]> mazeRows = new List<MazeCell[]>();
    private static Dictionary<int, List<MazeCell>> mazeCellSets = new Dictionary<int, List<MazeCell>>();

    void Start()
    {
        numOfRows = 0;
        numOfSets = 0;
    }

    private class MazeCell
    {
        // ID of the set that this cell belongs to in Eller's algorithm
        private int setID;

        public MazeCell(int pSetID)
        {
            setID = pSetID;
        }

        public int GetSetID()
        {
            return setID;
        }

        public void UpdateSetID(int newID)
        {
            setID = newID;
        }
    }

    private class Wall
    {
        private int xPosition;
        private int zPosition;

        public Wall(int pX, int pZ)
        {
            xPosition = pX;
            zPosition = pZ;
        }

        public int GetXPosition()
        {
            return xPosition;
        }

        public int GetZPosition()
        {
            return zPosition;
        }
    }

    public void GenerateNextRow()
    {
        // Add row floor and outer maze boundaries.
        Instantiate(mazeRowFloor, new Vector3(100 + (numOfRows * 10), 0, 10), Quaternion.identity, maze);
        Transform westMazeBoundary = Instantiate(nsMazeBoundary, new Vector3(105 + (numOfRows * 10), 0, 90), Quaternion.identity, maze);
        westMazeBoundary.GetComponent<BoundaryController>().maze = this;
        Transform eastMazeBoundary = Instantiate(nsMazeBoundary, new Vector3(105 + (numOfRows * 10), 0, 10), Quaternion.identity, maze);
        eastMazeBoundary.GetComponent<BoundaryController>().maze = this;
        Transform ceilingBoundary = Instantiate(ceilingMazeBoundary, new Vector3(105 + (numOfRows * 10), 50, 50), Quaternion.Euler(90, 0, 0), maze);
        ceilingBoundary.GetComponent<BoundaryController>().maze = this;


        if (numOfRows == 0)
        {
            GenerateFirstRow();
        }
        else
        {
            // Create north-south-oriented walls to be added to Maze Cells.
            // nSWalls[0] is the west-most wall and is the western outer wall of the row. 
            // nSWalls[8] is the east-most wall and is the eastern outer wall of the row.
            Wall[] nSWalls = new Wall[9];
            for (int i = 0; i < 9; i++)
            {
                nSWalls[i] = new Wall(105 + (numOfRows * 10), 90 - (i * 10));
            }

            // Create east-west oriented walls to be added to Maze Cells.
            // eWWalls[0] is on the south edge of the west-most cell.
            Wall[] eWWalls = new Wall[8];
            for (int i = 0; i < 8; i++)
            {
                  eWWalls[i] = new Wall(100 + (numOfRows * 10), 85 - (i * 10));
            }

            // Create row of 8 new maze cells and add walls on west, east, and south edges.
            MazeCell[] newRow = new MazeCell[8];
            mazeRows.Add(newRow);

            for (int i = 0; i < 8; i++){
                MazeCell newCell = new MazeCell(numOfSets);
                newRow[i] = newCell;
                List<MazeCell> cellsInSet = new List<MazeCell>();
                cellsInSet.Add(newCell);
                mazeCellSets[numOfSets] = cellsInSet;
                numOfSets++;
            }

            // Apply Eller's Algorithm: merge cells within row, then between new row and previous row.
            MergeWithinRow(newRow, nSWalls);
            MergeBetweenRows(newRow, mazeRows[numOfRows - 1], eWWalls);

            // Instantiate remaining north-south-oriented walls.
            for (int i = 0; i < 9; i++)
            {
                Wall w = nSWalls[i];
                if (w != null)
                {
                    Instantiate(nsMazeWall, new Vector3(w.GetXPosition(), 0, w.GetZPosition()), Quaternion.identity, maze);

                }
            }

            // Instantiate remaining east-west-oriented walls.
            for (int i = 0; i < 8; i++)
            {
                Wall w = eWWalls[i];
                if (w != null)
                {
                    Instantiate(ewMazeWall, new Vector3(w.GetXPosition(), 0, w.GetZPosition()), Quaternion.Euler(0, 90, 0), maze);

                }
            }

            // Instantiate a projectile in the center of each cell
            for (int i = 0; i < 8; i++)
            {
                Instantiate(projectile, new Vector3(105 + (numOfRows * 10), 1, 85 - (i * 10)), Quaternion.identity, maze);
            }

            numOfRows++;
        }
    }

    public void GenerateLastRow()
    {
        // Add row floor and outer maze boundaries, including an east-west-oriented boundary on the northern side of the maze.
        Instantiate(mazeRowFloor, new Vector3(100 + (numOfRows * 10), 0, 10), Quaternion.identity, maze);
        Transform westMazeBoundary = Instantiate(nsMazeBoundary, new Vector3(105 + (numOfRows * 10), 0, 90), Quaternion.identity, maze);
        westMazeBoundary.GetComponent<BoundaryController>().maze = this;
        Transform eastMazeBoundary = Instantiate(nsMazeBoundary, new Vector3(105 + (numOfRows * 10), 0, 10), Quaternion.identity, maze);
        eastMazeBoundary.GetComponent<BoundaryController>().maze = this;
        Transform ceilingBoundary = Instantiate(ceilingMazeBoundary, new Vector3(105 + (numOfRows * 10), 50, 50), Quaternion.Euler(90, 0, 0), maze);
        ceilingBoundary.GetComponent<BoundaryController>().maze = this;
        Transform northBoundary = Instantiate(ewMazeBoundary, new Vector3(110 + (numOfRows * 10), 0, 50), Quaternion.identity, maze);
        northBoundary.GetComponent<BoundaryController>().maze = this;

        // Add one more row, again merging sets, in which each cell has a northern wall and all sets are accessible.
        Wall[] nSWalls = new Wall[9];
        for (int i = 0; i < 9; i++)
        {
            nSWalls[i] = new Wall(105 + (numOfRows * 10), 90 - (i * 10));
        }

        // Create east-west oriented walls to be added to Maze Cells.
        // eWWalls[0] is on the south edge of the west-most cell.
        Wall[] eWWalls = new Wall[8];
        for (int i = 0; i < 8; i++)
        {
            eWWalls[i] = new Wall(100 + (numOfRows * 10), 85 - (i * 10));
        }

        // Create row of 8 new maze cells and add walls on west, east, and south edges.
        MazeCell[] newRow = new MazeCell[8];
        mazeRows.Add(newRow);

        for (int i = 0; i < 8; i++)
        {
            MazeCell newCell = new MazeCell(numOfSets);
            newRow[i] = newCell;
            List<MazeCell> cellsInSet = new List<MazeCell>();
            cellsInSet.Add(newCell);
            mazeCellSets[numOfSets] = cellsInSet;
            numOfSets++;
        }

        // Apply Eller's Algorithm: merge cells within row, then between new row and previous row.
        MergeWithinRow(newRow, nSWalls);
        MergeBetweenRows(newRow, mazeRows[numOfRows - 1], eWWalls);

        // Merge adjacent cells within the row if they are of different sets.
        for (int i = 0; i < 8; i++)
        {
            int currentSetID = newRow[i].GetSetID();
            if (i < 7)
            {
                if (currentSetID != newRow[i + 1].GetSetID())
                {
                    // Merge with the cell to the east.
                    nSWalls[i + 1] = null;
                    // Add the entire set of the east cell to the set of the current cell
                    MazeCell eastNeighbor = newRow[i + 1];
                    int neighborSetID = eastNeighbor.GetSetID();

                    List<MazeCell> neighborSet = mazeCellSets[neighborSetID];
                    List<MazeCell> cellSet = mazeCellSets[currentSetID];
                    foreach (MazeCell c in neighborSet)
                    {
                        c.UpdateSetID(currentSetID);
                        cellSet.Add(c);
                    }
                    mazeCellSets.Remove(neighborSetID);
                }
            }
            if (i > 0)
            {
                if (currentSetID != newRow[i - 1].GetSetID())
                {
                    // Merge with the cell to the west.
                    nSWalls[i] = null;
                    // Add the entire set of the west cell to the set of the current cell
                    MazeCell westNeighbor = newRow[i - 1];
                    int neighborSetID = westNeighbor.GetSetID();

                    List<MazeCell> neighborSet = mazeCellSets[neighborSetID];
                    List<MazeCell> cellSet = mazeCellSets[currentSetID];
                    foreach (MazeCell c in neighborSet)
                    {
                        c.UpdateSetID(currentSetID);
                        cellSet.Add(c);
                    }
                    mazeCellSets.Remove(neighborSetID);
                }
            }
        }

        // Instantiate remaining north-south-oriented walls.
        for (int i = 0; i < 9; i++)
        {
            Wall w = nSWalls[i];
            if (w != null)
            {
                Instantiate(nsMazeWall, new Vector3(w.GetXPosition(), 0, w.GetZPosition()), Quaternion.identity, maze);
            }
        }

        // Instantiate remaining east-west-oriented walls on south side of cells.
        for (int i = 0; i < 8; i++)
        {
            Wall w = eWWalls[i];
            if (w != null)
            {
                Instantiate(ewMazeWall, new Vector3(w.GetXPosition(), 0, w.GetZPosition()), Quaternion.Euler(0, 90, 0), maze);
            }
        }

        // Instantiate a projectile in the center of each cell
        for (int i = 0; i < 8; i++)
        {
            Instantiate(projectile, new Vector3(105 + (numOfRows * 10), 1, 85 - (i * 10)), Quaternion.identity, maze);
        }

        numOfRows++;

        // Instantiate walls on the north side of each cell to close the maze.
        for (int i = 0; i < 8; i++)
        {
            Instantiate(ewMazeWall, new Vector3(100 + (numOfRows * 10), 0, 85 - (i * 10)), Quaternion.Euler(0, 90, 0), maze);
        }

        mazeRowEntrance.gameObject.SetActive(false);
        mazeCompleted = true;
    }

    public bool GetMazeCompleted()
    {
        return mazeCompleted;
    }

    private void GenerateFirstRow()
    {
        // Same as GenerateNextRow, except it doesn't add southern walls to each cell and doesn't merge cells from the previous row.

        // Create north-south-oriented walls to be added to Maze Cells.
        // nsWalls[0] is the west-most wall and is the western outer wall of the row. 
        // nsWalls[8] is the east-most wall and is the eastern outer wall of the row.
        Wall[] nSWalls = new Wall[9];
        for (int i = 0; i < 9; i++)
        {
            nSWalls[i] = new Wall(105 + (numOfRows * 10), 90 - (i * 10));
        }

        // Create row of 8 new maze cells and add walls on west and east edges.
        MazeCell[] newRow = new MazeCell[8];
        mazeRows.Add(newRow);

        for (int i = 0; i < 8; i++)
        {
            MazeCell newCell = new MazeCell(numOfSets);
            newRow[i] = newCell;
            List<MazeCell> cellsInSet = new List<MazeCell>();
            cellsInSet.Add(newCell);
            mazeCellSets[numOfSets] = cellsInSet;
            numOfSets++;
        }

        // Apply beginning of Eller's algorithm.
        MergeWithinRow(newRow, nSWalls);

        // Instantiate remaining walls.
        foreach (Wall w in nSWalls)
        {
            if (w != null)
            {
                Instantiate(nsMazeWall, new Vector3(w.GetXPosition(), 0, w.GetZPosition()), Quaternion.identity, maze);

            }
        }

        // Instantiate a projectile in the center of each cell
        for (int i = 0; i < 8; i++)
        {
            Instantiate(projectile, new Vector3(105 + (numOfRows * 10), 1, 85 - (i * 10)), Quaternion.identity, maze);
        }

        numOfRows++;
    }

    private void MergeWithinRow(MazeCell[] row, Wall[] nSWalls)
    {
        // Randomly choose number of cells to merge in range [1, 7).
        System.Random rand = new System.Random();
        int numToMerge = rand.Next(1, 7);

        // Randomly choose cells to merge. May choose same cell multiple times.
        for (int i = 0; i < numToMerge; i++)
        {
            int cellPosition = rand.Next(0, 8);
            MazeCell cellToMerge = row[cellPosition];

            // Randomly decide whether to merge with west cell or east cell (0 means west cell, 1 means east cell). If the cellToMerge is an edge cell (i.e. one of its 
            // walls is an outer wall), merge with the only adjacent cell.
            int westOrEast = rand.Next(0,2);

            if (cellPosition == 0 || (cellPosition < 7 && westOrEast == 1))
            {
                // Merge with the cell to the east.
                nSWalls[cellPosition + 1] = null;
                // Entire set of the cell to the east of cellToMerge is added to the cellToMerge's set.
                MazeCell eastNeighbor = row[cellPosition + 1];
                int neighborSetID = eastNeighbor.GetSetID();
                int cellSetID = cellToMerge.GetSetID();

                // If the eastNeighbor is already in the same set as the cellToMerge, do not remove the wall between them to avoid creating loops.
                if (neighborSetID != cellSetID)
                {
                    List<MazeCell> neighborSet = mazeCellSets[neighborSetID];
                    List<MazeCell> cellSet = mazeCellSets[cellSetID];
                    foreach (MazeCell c in neighborSet)
                    {
                        c.UpdateSetID(cellSetID);
                        cellSet.Add(c);
                    }
                    mazeCellSets.Remove(neighborSetID);
                }
            }
            else if (cellPosition == 7 || (cellPosition > 0 && westOrEast == 0))
            {
                // Merge with the cell to the west.
                nSWalls[cellPosition] = null;
                // Entire set of cell to the west of cellToMerge is added to the cellToMerge's set.
                MazeCell westNeighbor = row[cellPosition - 1];
                int neighborSetID = westNeighbor.GetSetID();
                int cellSetID = cellToMerge.GetSetID();

                // If the westNeighbor is already in the same set as the cellToMerge, do not remove the wall between them to avoid creating loops.
                if (neighborSetID != cellSetID)
                {
                    List<MazeCell> neighborSet = mazeCellSets[neighborSetID];
                    List<MazeCell> cellSet = mazeCellSets[cellSetID];
                    foreach (MazeCell c in neighborSet)
                    {
                        c.UpdateSetID(cellSetID);
                        cellSet.Add(c);
                    }
                    mazeCellSets.Remove(neighborSetID);
                }
            }

        } 
    }

    private void MergeBetweenRows(MazeCell[] currentRow, MazeCell[] previousRow, Wall[] eWWalls)
    {
        // Determine which cell sets are present in the previous row. Dictionary key is the set ID. Dictionary value is a list of the positions (int) of cells in the row
        // that are part of the set.
        Dictionary<int, List<int>> setsInRow = new Dictionary<int, List<int>>();
        for (int i = 0; i < 8; i++)
        {
            MazeCell c = previousRow[i];
            if (setsInRow.ContainsKey(c.GetSetID()))
            {
                List<int> cellsInSet = setsInRow[c.GetSetID()];
                cellsInSet.Add(i);
            } 
            else
            {
                List<int> cellsInSet = new List<int>();
                cellsInSet.Add(i);
                setsInRow[c.GetSetID()] = cellsInSet;
            }
        }

        // For each set in the previous row, randomly select one cell in the set in the previous row to merge to the adjacent cell in the current row.
        System.Random rand = new System.Random();
        foreach (List<int> set in setsInRow.Values)
        {
            int positionToMerge = set[rand.Next(0, set.Count)];

            // Merge the cell at this position with the cell in the adjacent position in the new row.
            eWWalls[positionToMerge] = null;
            MazeCell northNeighbor = currentRow[positionToMerge];
            int cellSetID = previousRow[positionToMerge].GetSetID();
            int neighborSetID = northNeighbor.GetSetID();

            // Add the entire set of the cell in the current (northern-most) row to the set of the cell in the previous row.
            // If the northNeighbor is already in the same set as the cellToMerge, do not remove the wall between them to avoid creating loops.
            if (neighborSetID != cellSetID)
            {
                List<MazeCell> neighborSet = mazeCellSets[neighborSetID];
                List<MazeCell> cellSet = mazeCellSets[cellSetID];
                foreach (MazeCell c in neighborSet)
                {
                    c.UpdateSetID(cellSetID);
                    cellSet.Add(c);
                }
                mazeCellSets.Remove(neighborSetID);
            }
        }
    }
}
