using UnityEngine;
using EzySlice;
using TMPro;

public class Slicer : MonoBehaviour
{
    public Transform cube;
    public Transform cutPlane;
    public Material crossMaterial;

    public int score;

    public TextMeshProUGUI scoreText;

    private void FixedUpdate()
    {
        var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);    
        transform.position = objPosition;

        scoreText.text = score.ToString();
    }

    private void OnCollisionEnter(Collision otherObj)
    {
        if (otherObj.gameObject.CompareTag("Cube"))
        {
            var angleForCut = Mathf.Atan2(transform.position.y, transform.position.x) * (180 / Mathf.PI);
            cutPlane.transform.eulerAngles = new Vector3(0, 0, angleForCut);


            SlicedHull hull = SliceObject(otherObj.gameObject, crossMaterial);

            if (hull != null)
            {
                GameObject bottom = hull.CreateLowerHull(otherObj.gameObject, crossMaterial);
                GameObject top = hull.CreateUpperHull(otherObj.gameObject, crossMaterial);
                AddHullComponents(bottom);
                AddHullComponents(top);
                Destroy(otherObj.gameObject);
                score++;
            }


        }
    }

    private void AddHullComponents(GameObject go)
    {
        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;
        go.AddComponent<Destroyer>();
        rb.AddExplosionForce(100, go.transform.position, 20);
        rb.AddForce(Vector3.back * 4, ForceMode.Impulse);
        rb.mass= 5f;
    }

    private SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        if (obj.GetComponent<MeshFilter>() == null)
            return null;

        return obj.Slice(cutPlane.position, cutPlane.up, crossSectionMaterial);
    }

}
