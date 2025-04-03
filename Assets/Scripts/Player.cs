using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class Player : MonoBehaviour
{
    [Header("Player Settings")] [SerializeField]
    private float _health;

    public float _maxHealth = 100f;
    public float _speed = 3f;
    public float _jumpForce = 5f;
    [SerializeField] private Rigidbody _rb;
    [Header("Weapon Settings")] private Weapon _currentWeapon;

    [Header("Perks")] private List<Perk> _perks = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _health = _maxHealth;
        _rb = GetComponent<Rigidbody>();
        _currentWeapon = GetComponentInChildren<Weapon>();
    }

    // Update is called once per frame
    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var movement = new Vector3(horizontal, 0, vertical);
        movement.Normalize();
        movement *= _speed * Time.deltaTime;
        _rb.MovePosition(transform.position + movement);
        if (Input.GetButtonDown("Jump")) _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }
}