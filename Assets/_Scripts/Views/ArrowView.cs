using UnityEngine;

public class ArrowView : MonoBehaviour
{
    [SerializeField] private GameObject arrowHead;
    [SerializeField] private LineRenderer lineRenderer;
    private Vector3 startPosition;

    //NOTE:
    //CHANGE ARROW HEAD PIVOT TO BE AT THE TIP IF I WANT THE ARROW TIP TO BE AT MY MOUSE

    private void Update()
    {
        Vector3 endPosition = MouseUtil.GetMousePositionInWorldSpace();
        Vector3 direction = -(startPosition - arrowHead.transform.position).normalized; //Calc direction
        lineRenderer.SetPosition(1, endPosition - direction * 0.5f);    //Why 0.5f?
        arrowHead.transform.position = endPosition;
        arrowHead.transform.right = direction;
    }

    private void SetupArrow(Vector3 startPosition)
    { 
        this.startPosition = startPosition;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, MouseUtil.GetMousePositionInWorldSpace());
    }
}
