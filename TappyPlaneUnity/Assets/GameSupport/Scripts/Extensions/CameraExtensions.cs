using UnityEngine;

internal static class CameraExtensions
{
	public static bool IsVisible(this Camera camera, Bounds bounds)
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		return GeometryUtility.TestPlanesAABB(planes, bounds);
	}


    public static float FrustumHeight(this Camera camera, float distance)
    {
        if (camera.orthographic)
        {
            return camera.orthographicSize * 2.0f;
        }
        else
        {
            return 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }
    }


    public static float FrustumWidth(this Camera camera, float distance)
    {
        return camera.FrustumHeight(distance) * camera.aspect;
    }
}

