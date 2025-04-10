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

    [Header("Camera Settings")] public Transform cameraTransform;

    public float mouseSensitivity = 100f;

    [Header("Weapon Settings")] public List<Weapon> weapons = new();

    private int _currentWeaponIndex;

    [Header("Perks")] private List<Perk> _perks = new();

    private float _xRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _health = _maxHealth;
        _rb = GetComponent<Rigidbody>();

        // Initialize weapons
        if (weapons.Count > 0)
        {
            _currentWeaponIndex = 0;
            ActivateWeapon(_currentWeaponIndex);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleWeaponSwitching();
    }

    private void HandleMovement()
    {
        // Get input
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        // Calculate movement direction based on camera orientation
        var forward = new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z);
        var right = new Vector3(cameraTransform.right.x, 0f, cameraTransform.right.z);

        // Combine input with camera directions
        var movement = (forward * vertical + right * horizontal) * _speed * Time.deltaTime;

        // Move the player
        if (movement != Vector3.zero)
            _rb.MovePosition(transform.position + movement);

        // Jump
        if (Input.GetButtonDown("Jump"))
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void HandleCamera()
    {
        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleWeaponSwitching()
    {
        // Scroll wheel input
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            _currentWeaponIndex = (_currentWeaponIndex + 1) % weapons.Count;
            ActivateWeapon(_currentWeaponIndex);
        }
        else if (scroll < 0f)
        {
            _currentWeaponIndex = (_currentWeaponIndex - 1 + weapons.Count) % weapons.Count;
            ActivateWeapon(_currentWeaponIndex);
        }

        // Number key input
        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivateWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivateWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivateWeapon(2);
    }

    private void ActivateWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;

        for (var i = 0; i < weapons.Count; i++) weapons[i].gameObject.SetActive(i == index);
    }
}