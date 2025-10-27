using UnityEngine;

public class GridManager : Monobehaviour
{
    [SerializeField] private float m_Rows;
    [SerializeField] private float m_Cols;

    void Start()
    {
        for (int i = 0; i < m_Rows; i += 0.5)
        {
            for (int j = 0; j < m_Cols; j++)
            {
                
            }
        }
    }
}
