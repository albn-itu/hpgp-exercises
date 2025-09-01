using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] Transform pointPrefab;

    [SerializeField, Range(10, 100)] int resolution = 10;

    [SerializeField] FunctionLibrary.FunctionName function = FunctionLibrary.FunctionName.Wave;

    [SerializeField, Min(0f)] float functionDuration = 1f, transitionDuration = 1f;

    public enum TransitionMode
    {
        Cycle,
        Random
    }

    [SerializeField] TransitionMode transitionMode;

    private Transform[] _points;

    private float _duration;

    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    void Awake()
    {
        _points = new Transform[resolution * resolution];

        float step = 2f / resolution;
        var scale = Vector3.one * step;
        for (int i = 0; i < _points.Length; i++)
        {
            Transform point = _points[i] = Instantiate(pointPrefab);
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
        _duration += Time.deltaTime;

        if (transitioning)
        {
            if (_duration >= transitionDuration)
            {
                _duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (_duration >= functionDuration)
        {
            _duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }

        if (transitioning)
        {
            UpdateFunctionTransition();
        }
        else
        {
            UpdateFunction();
        }
    }

    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle
            ? FunctionLibrary.GetNextFunctionName(function)
            : FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }

    void UpdateFunction()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);

        float time = Time.time;
        float step = 2f / resolution;
        for (int i = 0, x = 0, z = 0; i < _points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
            }

            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;
            _points[i].localPosition = f(u, v, time);
        }
    }

    void UpdateFunctionTransition()
    {
        FunctionLibrary.Function
            from = FunctionLibrary.GetFunction(transitionFunction),
            to = FunctionLibrary.GetFunction(function);
        float progress = _duration / transitionDuration;
        float time = Time.time;
        float step = 2f / resolution;
        for (int i = 0, x = 0, z = 0; i < _points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
            }

            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;
            _points[i].localPosition = FunctionLibrary.Morph(
                u, v, time, from, to, progress
            );
        }
    }
}