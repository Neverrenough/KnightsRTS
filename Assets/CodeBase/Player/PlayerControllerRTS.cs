using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControllerRTS : MonoBehaviour
{
    [SerializeField] private PlayerRTS player;
    [SerializeField] private SceneLoader sceneLoader;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float edgeThreshold;
    [SerializeField] private float dragSpeed = 1f;

    private Vector3 moveDirection;
    private bool isDragging = false;

    [SerializeField] private RectTransform selectionBox;
    private List<UnitControllerRTS> selectedUnits = new List<UnitControllerRTS>();
    private Vector2 selectionStart;
    private bool isSelecting = false;

    [SerializeField] private RectTransform topLeftCorner;
    [SerializeField] private RectTransform topRightCorner;
    [SerializeField] private RectTransform bottomLeftCorner;
    [SerializeField] private RectTransform bottomRightCorner;

    private void Start()
    {
        edgeThreshold = Mathf.Min(Screen.width, Screen.height) * 0.02f; // 2% от меньшего измерения экрана
    }

    private void Update()
    {
        UpdateSelectionCorners();
        CameraEdgeScroll();
        CameraDragMove();

        SelectionArea();
        SelectionUnit();

        RestartLevel();

        MoveCommand();
        StopCommand();
        PatrolCommand();
        AttackCommand();
    }

    private bool IsAlly(UnitControllerRTS unit)
    {
        return unit != null && unit.UnitStats.Team == player.Team;
    }

    private void RestartLevel()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            sceneLoader.LoadScene(currentSceneIndex);
        }
    }

    private void AttackCommand()
    {
        if (selectedUnits.Count > 0 && Input.GetMouseButtonDown(1)) // ПКМ
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if (hitCollider != null)
            {
                UnitRTS enemyUnit = hitCollider.GetComponent<UnitRTS>();
                BuildingRTS enemyBuilding = hitCollider.GetComponent<BuildingRTS>();

                if (enemyUnit != null) // Если кликнули по юниту
                {
                    foreach (var unit in selectedUnits)
                    {
                        if (unit != null)
                        {
                            unit.Attack(enemyUnit.transform);
                        }
                    }
                }
                else if (enemyBuilding != null) // Если кликнули по зданию
                {
                    foreach (var unit in selectedUnits)
                    {
                        if (unit != null)
                        {
                            unit.Attack(enemyBuilding.transform);
                        }
                    }
                }
                else // Если кликнули в пустую область, двигаем юнитов
                {
                    MoveSelectedUnits(mousePosition);
                }
            }
            else
            {
                MoveSelectedUnits(mousePosition);
            }
        }
    }

    private void MoveCommand()
    {
        if (selectedUnits.Count > 0 && Input.GetMouseButtonDown(1)) // ПКМ
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MoveSelectedUnits(mousePosition);
        }
    }

    /// <summary>
    /// Двигает всех выбранных юнитов.
    /// </summary>
    private void MoveSelectedUnits(Vector2 targetPosition)
    {
        for (int i = selectedUnits.Count - 1; i >= 0; i--) // Обходим список с конца, чтобы удалять мертвых юнитов
        {
            if (selectedUnits[i] == null)
            {
                selectedUnits.RemoveAt(i);
                continue;
            }

            selectedUnits[i].MoveTo(targetPosition);
        }
    }

    private void StopCommand()
    {
        if (selectedUnits.Count > 0 && Input.GetKeyDown(KeyCode.S))
        {
            foreach (var unit in selectedUnits)
            {
                if (unit != null)
                {
                    unit.Stop();
                }
            }
        }
    }

    private void PatrolCommand()
    {
        if (selectedUnits.Count > 0 && Input.GetKeyDown(KeyCode.P))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (var unit in selectedUnits)
            {
                if (unit != null)
                {
                    unit.Patrol(mousePosition);
                }
            }
        }
    }

    private void SelectionUnit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if (hitCollider != null)
            {
                UnitControllerRTS unit = hitCollider.GetComponentInParent<UnitControllerRTS>();
                if (IsAlly(unit))
                {
                    DeselectAllUnits();
                    selectedUnits.Add(unit);
                    unit.Select();
                }
            }
            else
            {
                DeselectAllUnits();
                isSelecting = true;
                selectionStart = Input.mousePosition;
                selectionBox.gameObject.SetActive(true);
            }
        }
    }

    private void SelectionArea()
    {
        if (isSelecting)
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 size = currentMousePosition - selectionStart;
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            selectionBox.anchoredPosition = selectionStart + size / 2;
        }

        if (Input.GetMouseButtonUp(0) && isSelecting)
        {
            isSelecting = false;
            selectionBox.gameObject.SetActive(false);

            Vector2 min = selectionStart;
            Vector2 max = Input.mousePosition;

            if (min.x > max.x) (min.x, max.x) = (max.x, min.x);
            if (min.y > max.y) (min.y, max.y) = (max.y, min.y);

            Collider2D[] colliders = Physics2D.OverlapAreaAll(Camera.main.ScreenToWorldPoint(min), Camera.main.ScreenToWorldPoint(max));

            DeselectAllUnits();

            foreach (Collider2D col in colliders)
            {
                UnitControllerRTS unit = col.GetComponentInParent<UnitControllerRTS>();
                if (IsAlly(unit))
                {
                    selectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }
    }

    private void DeselectAllUnits()
    {
        foreach (var unit in selectedUnits)
        {
            if (unit != null)
            {
                unit.Deselect();
            }
        }

        selectedUnits.Clear();
    }

    private void CameraEdgeScroll()
    {
        moveDirection = Vector3.zero;
        Vector3 mousePosition = Input.mousePosition;

        if (mousePosition.x >= Screen.width - edgeThreshold) moveDirection.x += 1;
        if (mousePosition.x <= edgeThreshold) moveDirection.x -= 1;
        if (mousePosition.y >= Screen.height - edgeThreshold) moveDirection.y += 1;
        if (mousePosition.y <= edgeThreshold) moveDirection.y -= 1;

        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
    }

    private void CameraDragMove()
    {
        if (Input.GetMouseButtonDown(2)) // Средняя кнопка мыши
        {
            isDragging = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        if (Input.GetMouseButton(2) && isDragging)
        {
            Vector3 difference = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            transform.position -= difference * dragSpeed;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void UpdateSelectionCorners()
    {
        Vector2 size = selectionBox.sizeDelta;
        topLeftCorner.anchoredPosition = new Vector2(-size.x / 2, size.y / 2);
        topRightCorner.anchoredPosition = new Vector2(size.x / 2, size.y / 2);
        bottomLeftCorner.anchoredPosition = new Vector2(-size.x / 2, -size.y / 2);
        bottomRightCorner.anchoredPosition = new Vector2(size.x / 2, -size.y / 2);
    }
}
