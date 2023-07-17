using UnityEngine;

public class LockBullet : ProyectileBehavior
{
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private float _lockTime = 6;

    private Transform _player;
#if UNITY_EDITOR
    [Help("Time to stop locking, if 0 locks infinitely", UnityEditor.MessageType.None)]
#endif
    private bool _locking = true;

    protected override void Start()
    {
        base.Start();
        _player = FindObjectOfType<PlayerController>().transform;
        if(_lockTime != 0)
            Invoke(nameof(StopLock), _lockTime);
    }

    public override void Move(float speed)
    {
        _speed = speed;
        rb.velocity = transform.up * _speed;
    }

    protected override void Update()
    {
        base.Update();

        if(_locking)
            PointAtPlayer();

        rb.velocity = transform.up * _speed;
    }

    private void StopLock()
    {
        _locking = false;
    }

    protected virtual void PointAtPlayer()
    {
        Vector3 dir = _player.position - transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, _rotationSpeed * Time.deltaTime);
    }
}
