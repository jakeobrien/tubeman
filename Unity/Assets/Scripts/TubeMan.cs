using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TubeMan : MonoBehaviour 
{
	public event Action DidDie;
	public GameObject facePrefab;
	public TubemanUI tubemanUI;
	public int faceIndex;
	public bool isDead;
	public float health;
	public Color color;
	public int playerIndex;
	public int numberTorsoSections;
	public int numberArmSections;
	public int soulSectionIndex;
	public float bottomTorsoForce;
	public int armTorsoNode;
	public float topTorsoForce;
	public Range torsoBottomDuration;
	public Range torsoTopDuration;
	public Range torsoHoldDuration;
	public float torsoAngle;
	public float armAngle;
	public Range armInsideForce;
	public Range armOutsideForce;

	public TubeSection tubeSectionPrefab;
	public TubeSection rightArmSectionPrefab;
	public TubeSection leftArmSectionPrefab;

	private TubeSection[] _torsoSections;
	private TubeSection[] _leftArmSections;
	private TubeSection[] _rightArmSections;

	private void Start()
	{
		tubemanUI.Health = health = 1f;
		CreateTubeMan();
		StartCoroutine(Pulse());
		StartCoroutine(CheckDead());
	}

	private void CreateTubeMan()
	{
		var startPos = transform.position;
		TubeSection previousTubeSection = null;
		_torsoSections = new TubeSection[numberTorsoSections];
		for (int i = 0; i < numberTorsoSections; i++)
		{
			var pos = startPos + Vector3.up * i * tubeSectionPrefab.transform.localScale.y;
			pos += Vector3.forward * UnityEngine.Random.Range(-0.1f, 0.1f);
			pos += Vector3.right * UnityEngine.Random.Range(-0.1f, 0.1f);
			var tubeSection = Instantiate(tubeSectionPrefab, pos, Quaternion.identity);
			tubeSection.playerIndex = playerIndex;
			tubeSection.maxHealthColor = color;
			if (i == faceIndex)
			{
				var face = Instantiate(facePrefab, Vector3.zero, Quaternion.identity);
				face.transform.parent = tubeSection.transform;
				face.transform.localPosition = transform.forward * 0.5f;
			}
			if (previousTubeSection != null) 
			{
				tubeSection.joint.connectedBody = previousTubeSection.rigidbody;
			}
			else
			{
				ConfigureRootJoint(tubeSection.joint);
				tubeSection.joint.connectedAnchor = startPos;
			}
			SetJointLimits(tubeSection.joint, torsoAngle);
			_torsoSections[i] = tubeSection;
			previousTubeSection = tubeSection;
			if (i >= numberTorsoSections - 1)
			{
				 tubeSection.gameObject.layer = 8;
				 tubeSection.isDamager = true;
			}
		}
		TubeSection previousRightArmSection = null;
		TubeSection previousLeftArmSection = null;
		_rightArmSections = new TubeSection[numberTorsoSections];
		_leftArmSections = new TubeSection[numberTorsoSections];
		for (int i = 0; i < numberArmSections; i++)
		{
			var torsoSection = _torsoSections[armTorsoNode];
			var rightArmPos = torsoSection.transform.position + Vector3.right * (i+1) * rightArmSectionPrefab.transform.localScale.x;
			var leftArmPos = torsoSection.transform.position + Vector3.left * (i+1) * leftArmSectionPrefab.transform.localScale.x;
			var rightArmSection = Instantiate(rightArmSectionPrefab, rightArmPos, Quaternion.AngleAxis(90f, Vector3.forward));
			rightArmSection.playerIndex = playerIndex;
			rightArmSection.maxHealthColor = color;
			var leftArmSection = Instantiate(leftArmSectionPrefab, leftArmPos, Quaternion.AngleAxis(90f, Vector3.forward));
			leftArmSection.playerIndex = playerIndex;
			leftArmSection.maxHealthColor = color;
			if (previousRightArmSection != null) 
			{
				rightArmSection.joint.connectedBody = previousRightArmSection.rigidbody;
				leftArmSection.joint.connectedBody = previousLeftArmSection.rigidbody;
			}
			else
			{
				rightArmSection.joint.connectedBody = torsoSection.rigidbody;
				rightArmSection.joint.connectedAnchor = new Vector3(0.5f, 0f, 0f);
				leftArmSection.joint.connectedBody = torsoSection.rigidbody;
				leftArmSection.joint.connectedAnchor = new Vector3(-0.5f, 0f, 0f);
			}
			_rightArmSections[i] = rightArmSection;
			_leftArmSections[i] = leftArmSection;
			SetJointLimits(rightArmSection.joint, armAngle);
			SetJointLimits(leftArmSection.joint, armAngle);
			previousRightArmSection = rightArmSection;
			previousLeftArmSection = leftArmSection;
			if (i >= numberArmSections - 2)
			{
				 rightArmSection.gameObject.layer = 8;
				 rightArmSection.isDamager = true;
				 leftArmSection.gameObject.layer = 8;
				 leftArmSection.isDamager = true;
			}
		}
	}

	private void SetJointLimits(ConfigurableJoint joint, float angle)
	{
		var limit = joint.lowAngularXLimit;
		limit.limit = -angle;
		joint.lowAngularXLimit = limit;
		limit = joint.highAngularXLimit;
		limit.limit = -angle;
		joint.highAngularXLimit = limit;
		limit = joint.angularYLimit;
		limit.limit = -angle;
		joint.angularYLimit = limit;
		limit = joint.angularZLimit;
		limit.limit = -angle;
		joint.angularZLimit = limit;
	}

	private IEnumerator Pulse()
	{
		while (true)
		{
			var bottomDuration = torsoBottomDuration.RandomInRange;
			var topDuration = torsoTopDuration.RandomInRange;
			var holdDuration = torsoHoldDuration.RandomInRange;
			for (int i = 0; i < _torsoSections.Length; i++)
			{
				var section = _torsoSections[i];
				if (section.GetComponent<ConfigurableJoint>() == null) break;
				float normalizedSection = (float)i / (float)numberTorsoSections;
				var duration = GetDurationForNormalizedSection(normalizedSection, bottomDuration, topDuration);
				if (i == armTorsoNode) 
				{
					var remainingTime = duration + holdDuration;
					for (int j = i + 1; j < _torsoSections.Length; j++)
					{
						float ns = (float)i / (float)numberTorsoSections;
						remainingTime += GetDurationForNormalizedSection(ns, bottomDuration, topDuration);
					}
					StartCoroutine(PulseArms(remainingTime));
				}
				yield return PulseSection(i, normalizedSection, duration);
			}
			// yield return PulseSection(_torsoSections.Length-1, 1f, holdDuration);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.7f));
		}
	}

	private float GetDurationForNormalizedSection(float normalizedSection, float bottomDuration, float topDuration)
	{
		return Mathf.Lerp(bottomDuration, topDuration, normalizedSection);
	}

	private IEnumerator PulseSection(int index, float normalizedSection, float duration)
	{
		var torsoForce = Mathf.Lerp(bottomTorsoForce, topTorsoForce, normalizedSection);
		var elapsed = 0f;
		var section = _torsoSections[index];
		while (elapsed < duration)
		{
			section.rigidbody.AddForce(torsoForce * Vector3.up, ForceMode.Force);
			elapsed += Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator PulseArms(float remainingTime)
	{
		var durationPerSection = remainingTime / (float)numberArmSections;
		var sectionIndex = 0;			
		var insideForce = armInsideForce.RandomInRange;
		var outsideForce = armOutsideForce.RandomInRange;
		while (sectionIndex < numberArmSections)
		{
			var sections = new List<TubeSection> { _rightArmSections[sectionIndex], _leftArmSections[sectionIndex] };
			float normalizedSection = (float)sectionIndex / (float)numberTorsoSections;
			var force = Mathf.Lerp(insideForce, outsideForce, normalizedSection);
			yield return ApplyForceToSections(sections, transform.forward * force, durationPerSection);
			
			// yield return WaitForSeconds(durationPerSection);
			sectionIndex++;
		}
	}

	private IEnumerator ApplyForceToSection(TubeSection section, Vector3 force, float duration)
	{
		var elapsed = 0f;
		while (elapsed < duration)
		{
			section.rigidbody.AddForce(force, ForceMode.Force);
			elapsed += Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator ApplyForceToSections(List<TubeSection> sections, Vector3 force, float duration)
	{
		var elapsed = 0f;
		while (elapsed < duration)
		{
			foreach (var section in sections)
			{
				section.rigidbody.AddForce(force, ForceMode.Force);
			}
			elapsed += Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator CheckDead()
	{
		while (!isDead)
		{
			for (int i = 0; i < _torsoSections.Length; i++)
			{
				var section = _torsoSections[i];

				if (section.CurrentHealth <= 0)
				{
					health = (float)i / (float)numberTorsoSections;
					tubemanUI.Health = health;
					if (i <= soulSectionIndex) isDead = true;
					if (DidDie != null) DidDie();
					break;
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void ConfigureRootJoint(ConfigurableJoint joint)
	{
		joint.connectedAnchor = Vector3.zero;
		joint.anchor = Vector3.zero;
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
	}

}
