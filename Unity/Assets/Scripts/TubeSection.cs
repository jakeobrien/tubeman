using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeSection : MonoBehaviour 
{
	public int playerIndex;
	public int maxHealth;
	public int damageThreshold;
	public AnimationCurve damageVelocityCurve;
	public Color maxHealthColor;
	public Color minHealthColor;
	public Color damageColor;

	public ConfigurableJoint joint;
	public Rigidbody rigidbody;
	public LayerMask damagingLayers;
	public bool isDamager;
	private Renderer _renderer;
	private Color _currentHealthTint;
	private bool _isAnimatingDamage;

	private int _currentHealth;
	private int CurrentHealth
	{
		get { return _currentHealth; }
		set
		{
			if (_currentHealth == value) return;
			if (value < _currentHealth) StartCoroutine(AnimateDamage());
			_currentHealth = value;
			TintForHealth();
			if (_currentHealth == 0)
			{
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				GetComponent<Rigidbody>().mass = 1f;
				Destroy(GetComponent<ConfigurableJoint>());
			}
		}
	}

	private void Awake()
	{
		joint = GetComponent<ConfigurableJoint>();
		rigidbody = GetComponent<Rigidbody>();
		_renderer = GetComponent<Renderer>();
	}

	private void Start()
	{
		CurrentHealth = maxHealth;
	}

	private void OnCollisionEnter(Collision coll)
	{
		// return;
		if (isDamager) return;
		// Debug.Log("2: " + coll.gameObject.layer);
		if (!damagingLayers.ContainsLayer(coll.gameObject.layer)) return;
		var tube = coll.gameObject.GetComponent<TubeSection>();
		if (tube == null || tube.playerIndex == playerIndex) return;
		var damage = Mathf.RoundToInt(damageVelocityCurve.Evaluate(coll.relativeVelocity.magnitude));
		// Debug.Log("3: " + damage);
		if (damage >= damageThreshold) CurrentHealth -= damage;
	}

	private IEnumerator AnimateDamage()
	{
		_isAnimatingDamage = true;
		var elapsed = 0f;
		var time = 1f;
		var mat = _renderer.material;
		while (elapsed < time)
		{
			var t = elapsed / time;
			if (t < 0.5f)
			{
				mat.color = Color.Lerp(mat.color, damageColor, Mathf.InverseLerp(0f, 0.5f, t));
			}
			else
			{
				mat.color = Color.Lerp(damageColor, _currentHealthTint, Mathf.InverseLerp(0.5f, 1f, t));
			}
			elapsed += Time.deltaTime;
			yield return null;
		}
		_isAnimatingDamage = false;
	}

	private void TintForHealth()
	{
		_currentHealthTint = Color.Lerp(minHealthColor, maxHealthColor, (float)_currentHealth / (float)maxHealth);
		if (!_isAnimatingDamage) _renderer.material.color = _currentHealthTint;
	}

}
