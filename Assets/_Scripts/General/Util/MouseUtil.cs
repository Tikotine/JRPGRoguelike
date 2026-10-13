using UnityEngine;

public static class MouseUtil
{
    private static Camera mainCamera;

    public static Vector3 GetMousePositionInWorldSpace(float zValue = 0f)    //Takes in a value in case we want it to be higher/lower in the axis
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Plane dragPlane = new(mainCamera.transform.forward, new Vector3(0, 0, zValue));   //Useful so you can say which plane you want to drag the card along
        Ray ray = mainCamera.ScreenPointToRay(PlayerInput.Instance.GetPointerScreenPosition());
        if (dragPlane.Raycast(ray, out float distance))     //Return the point if the ray hits the point on the plane
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}
