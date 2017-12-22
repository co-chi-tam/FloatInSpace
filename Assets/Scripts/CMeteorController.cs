using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CMeteorController : MonoBehaviour {

	[Header("Control")]
	[SerializeField]	protected Transform m_Astronaut;
	[SerializeField]	protected float m_MinRadius = 20f;
	[SerializeField]	protected Vector3 m_Target;
	protected Vector3 target {
		get { return this.m_Target; }
		set { this.m_Target = value; }
	}

	[Header("Data")]
	[SerializeField]	protected int m_DamageValue = 1;
	[SerializeField]	protected float m_FloatSpeed = 10f;
	[SerializeField]	protected float m_FloatDamping = 0.5f;
	[SerializeField]	protected float m_TurnSpeed = 10f;
	[SerializeField]	protected float m_TurnDamping = 0.5f;

	[Header("Target")]
	[SerializeField]	protected LayerMask m_TargetPlayerMask;

	[Header("Event")]
	public UnityEvent OnNearTarget;
	public UnityEvent OnExplosion;

	protected Transform m_Transform;
	protected bool m_IsNearestTarget = false;

	protected void Awake() {
		this.m_Transform = this.transform;
	}

	protected void Update() {
		if (CGameSetting.IsEndGame)
			return;
		this.CheckTarget ();
		this.Floating ();
		this.Turn ();
	}

	protected void OnTriggerEnter2D(Collider2D col) {
		if (this.m_TargetPlayerMask == (this.m_TargetPlayerMask | (1 << col.gameObject.layer))) {
			var spaceObj = col.GetComponent<ISpaceObject> ();
			spaceObj.ApplyDamage (-this.m_DamageValue);
		}
		if (this.OnExplosion != null) {
			this.OnExplosion.Invoke ();
		}
	}

	public void Floating() {
		var value = this.m_Transform.up;
		var speed = this.m_FloatSpeed;
		var movePosition = this.m_Transform.position + (value * speed * Time.deltaTime);
		this.m_Transform.position = Vector3.Lerp (this.m_Transform.position, movePosition, this.m_FloatDamping);
	}

	public void Turn() {
		var direction = this.m_Target - this.m_Transform.position;
		var angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
		var value = Quaternion.AngleAxis (angle - 90f, Vector3.forward);
		this.m_Transform.rotation = Quaternion.Lerp (this.m_Transform.rotation, value, this.m_TurnDamping);
	}

	public void CheckTarget() {
		var direction = this.m_Target - this.m_Transform.position;
		if (direction.sqrMagnitude <= 0.1f) {
			if (this.OnNearTarget != null) {
				this.OnNearTarget.Invoke ();
			}
			this.m_IsNearestTarget = true;
		}
	}

	public void Reset() {
		if (this.m_Astronaut == null)
			return;
		var direction = this.m_Astronaut.transform.position - this.m_Transform.position;
		var nextTarget = direction.normalized * this.m_MinRadius + this.m_Astronaut.transform.position;
		this.m_Target = new Vector3 (nextTarget.x, nextTarget.y, 0f);
		this.m_IsNearestTarget = false;
	}

}
