using UnityEngine;


[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class JellyMesh : MonoBehaviour
{
    public float Intensity = 1f;
    public float Mass = 1f;
    public float stiffness = 1f;
    public float damping = 0.75f;


    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private MeshFilter _meshFilter;


    private Mesh _originalMesh;
    private Mesh _meshClone;
    private JellyVertex[] _jv;
    private Vector3[] _vertexArray;
    public void Start()
    {
        if(_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }
        if (_renderer == null) 
        {
            _renderer = GetComponent<MeshRenderer>();
        }
        _originalMesh = _meshFilter.sharedMesh;
        _meshClone = Instantiate(_originalMesh);
        _meshFilter.sharedMesh = _meshClone;
        _jv = new JellyVertex[_meshClone.vertices.Length];
        for (int i = 0; i < _meshClone.vertices.Length; i++)
        {
            _jv[i] = new JellyVertex(i, transform.TransformPoint(_meshClone.vertices[i]));
        }
    }
    void FixedUpdate()
    {
        _vertexArray = _originalMesh.vertices;
        for (int i = 0; i < _jv.Length; i++)
        {
            Vector3 target = transform.TransformPoint(_vertexArray[_jv[i].ID]);
            float intensity = (1 - (_renderer.bounds.max.y - target.y) / _renderer.bounds.size.y) * Intensity;
            _jv[i].Shake(target, Mass, stiffness, damping);
            target = transform.InverseTransformPoint(_jv[i].Position);
            _vertexArray[_jv[i].ID] = Vector3.Lerp(_vertexArray[_jv[i].ID], target, intensity);
        }
        _meshClone.vertices = _vertexArray;
    }
    public class JellyVertex
    {
        public int ID;
        public Vector3 Position;
        public Vector3 velocity, Force;
        public JellyVertex(int _id, Vector3 _pos)
        {
            ID = _id;
            Position = _pos;
        }
        public void Shake(Vector3 target, float m, float s, float d)
        {
            Force = (target - Position) * s;
            velocity = (velocity + Force / m) * d;
            Position += velocity;
            if ((velocity + Force + Force / m).magnitude < 0.001f)
                Position = target;
        }
    }
}