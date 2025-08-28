using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] Transform pointPrefab;
    
    [SerializeField, Range(10, 100)] int resolution = 10;

    private Transform[] _points;

    void Awake()
    {
	    _points = new Transform[resolution];
	    
		float step = 2f / resolution;
		var position = Vector3.zero;
		var scale = Vector3.one * step;
		for (int i = 0; i < _points.Length; i++) {
			Transform point = _points[i] = Instantiate(pointPrefab);
			position.x = (i + 0.5f) * step - 1f;
			point.localPosition = position;
			point.localScale = scale;
			point.SetParent(transform, false);
		}
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    float time = Time.time;
	    for (int i = 0; i < _points.Length; i++)
	    {
		    Transform point = _points[i];
		    Vector3 position = point.localPosition;
			position.y = Mathf.Sin(Mathf.PI * (position.x + time));
		    point.localPosition = position;
	    }
    }
}
