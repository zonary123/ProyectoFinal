using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class Player : MonoBehaviour{
	[Header("Player Settings")] [SerializeField]
	private float _health;

	public float _maxHealth = 100f;
	public float _speed = 5f;
	public float _sprintSpeed = 10f;
	public float _jumpForce = 5f;
	[SerializeField] private Rigidbody _rb;

	[Header("Camera Settings")] public Transform cameraTransform;

	public float mouseSensitivity = 200f;

	[Header("Weapon Settings")] public List<Weapon> weapons = new();

	public Transform weaponMount; // Punto donde se montará el arma

	private int _currentWeaponIndex;

	private bool _isGrounded;
	[Header("Perks")] private List<Perk> _perks = new();

	private float _xRotation;
	private bool isdead;

	private void Awake(){
		// Cargar un arma predeterminada desde Resources
		var defaultWeaponPrefab = Resources.Load<Weapon>("Weapons/Pistol");
		if (defaultWeaponPrefab != null){
			var defaultWeapon = Instantiate(defaultWeaponPrefab, weaponMount.position, weaponMount.rotation, weaponMount);
			weapons.Add(defaultWeapon);
		}
		else{
			Debug.LogError("Default weapon not found in Resources/Weapons!");
		}

		var M16 = Resources.Load<Weapon>("Weapons/M16");
		if (M16 != null){
			var m16 = Instantiate(M16, weaponMount.position, weaponMount.rotation, weaponMount);
			weapons.Add(m16);
		}
		else{
			Debug.LogError("M16 weapon not found in Resources/Weapons!");
		}
	}

	private void Start(){
		_health = _maxHealth;
		_rb = GetComponent<Rigidbody>();
		GameManager.Instance.player = this;
		if (weapons == null || weapons.Count == 0){
			Debug.LogWarning("No weapons assigned to the player.");
			return;
		}

		// Inicializar el arma activa
		_currentWeaponIndex = 0;
		ActivateWeapon(_currentWeaponIndex);
		GameManager.Instance.UpdateAmmoText(weapons[_currentWeaponIndex]);
	}

	private void Update(){
		if (isdead) return;
		if (_health <= 0){
			isdead = true;
			GameManager.Instance.GameOver();
		}

		HandleMovement();
		HandleCamera();
		HandleWeaponSwitching();
		HandleShooting();
		HandleReloading();
	}

	private void HandleMovement(){
		var horizontal = Input.GetAxis("Horizontal");
		var vertical = Input.GetAxis("Vertical");

		var forward = new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z).normalized;
		var right = new Vector3(cameraTransform.right.x, 0f, cameraTransform.right.z).normalized;

		var speed = Input.GetKey(KeyCode.LeftShift) ? _sprintSpeed : _speed;
		var movement = (horizontal * right + vertical * forward).normalized * speed;

		if (movement != Vector3.zero)
			_rb.MovePosition(_rb.position + movement * Time.fixedDeltaTime);

		// Realizar un Raycast hacia abajo para verificar si está en el suelo
		_isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

		if (Input.GetButtonDown("Jump") && _isGrounded){
			_rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
			_isGrounded = false; // Evitar más saltos hasta que toque el suelo
		}
	}

	private void HandleCamera(){
		var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		_xRotation -= mouseY;
		_xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

		cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
		transform.Rotate(Vector3.up * mouseX);
	}

	private void HandleWeaponSwitching(){
		var scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll > 0f){
			_currentWeaponIndex = (_currentWeaponIndex + 1) % weapons.Count;
			ActivateWeapon(_currentWeaponIndex);
		}
		else if (scroll < 0f){
			_currentWeaponIndex = (_currentWeaponIndex - 1 + weapons.Count) % weapons.Count;
			ActivateWeapon(_currentWeaponIndex);
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) ActivateWeapon(0);
		if (Input.GetKeyDown(KeyCode.Alpha2)) ActivateWeapon(1);
		if (Input.GetKeyDown(KeyCode.Alpha3)) ActivateWeapon(2);
	}

	private void HandleShooting(){
		if (Input.GetButton("Fire1")) FireWeapon();
	}

	private void HandleReloading(){
		if (Input.GetKeyDown(KeyCode.R)) ReloadWeapon();
	}

	private void FireWeapon(){
		if (_currentWeaponIndex < 0 || _currentWeaponIndex >= weapons.Count){
			Debug.LogWarning("No active weapon to fire.");
			return;
		}

		var activeWeapon = weapons[_currentWeaponIndex];
		var firePosition = cameraTransform.position;
		var fireDirection = cameraTransform.forward;

		Debug.Log($"Firing weapon: {activeWeapon.name}");
		activeWeapon.Fire(firePosition, fireDirection);
		GameManager.Instance.UpdateAmmoText(weapons[_currentWeaponIndex]);
	}

	private void ReloadWeapon(){
		if (_currentWeaponIndex < 0 || _currentWeaponIndex >= weapons.Count) return;

		var activeWeapon = weapons[_currentWeaponIndex];
		activeWeapon.Reload();
		GameManager.Instance.UpdateAmmoText(weapons[_currentWeaponIndex]);
	}

	private void ActivateWeapon(int index){
		if (index < 0 || index >= weapons.Count) return;

		for (var i = 0; i < weapons.Count; i++){
			var weapon = weapons[i];
			weapon.gameObject.SetActive(i == index);

			if (i == index){
				// Posicionar el arma activa en el punto de montaje
				weapon.transform.SetParent(weaponMount);
				weapon.transform.localPosition = Vector3.zero; // Posición local en el punto de montaje
				weapon.transform.localRotation = Quaternion.identity; // Rotación local en el punto de montaje
			}
		}

		GameManager.Instance.UpdateAmmoText(weapons[_currentWeaponIndex]);
	}

	public void TakeDamage(int damage){
		if (GameManager.Instance.isDebug)
			Debug.Log("Player Take damage: " + damage);

		_health -= damage;
		if (_health < 0) _health = 0;
	}
}