using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class Player : MonoBehaviour
{
    
    [Header("Player Settings")] [SerializeField]
    private float _health;

    [SerializeField] private float _maxHealth;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;

    [Header("Weapon Settings")] private Weapon _currentWeapon;

    [Header("Perks")] private List<Perk> _perks = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}