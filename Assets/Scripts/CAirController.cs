using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CAirController : MonoBehaviour {

	[Header("Target")]
	[SerializeField]	protected LayerMask m_TargetPlayerMask;
	[SerializeField]	protected Transform m_Astronaut;
	[SerializeField]	protected float m_MinRadius = 10f;

	[Header("Target")]
	[SerializeField]	protected float m_LifeTime = 10f;
	[SerializeField]	protected float m_LifeTimeInterval = 10f;

	[Header("Air")]
	[SerializeField]	protected int m_AirValue = 1;

	[Header("Event")]
	public UnityEvent OnApplyAir;

	protected Transform m_Transform;

	protected void Awake() {
		this.m_Transform = this.transform;
	}

	protected void Update() {
		if (CGameSetting.IsEndGame)
			return;
		if (this.m_LifeTime > 0f) {
			this.m_LifeTime -= Time.deltaTime;
		} else {
			var direction = this.m_Astronaut.transform.position - this.m_Transform.position;
			if (direction.sqrMagnitude > this.m_MinRadius * this.m_MinRadius) {
				this.Reset ();
			}
		}
	}

	protected void OnTriggerEnter2D(Collider2D col) {
		if (this.m_TargetPlayerMask == (this.m_TargetPlayerMask | (1 << col.gameObject.layer))) {
			var spaceObj = col.GetComponent<ISpaceObject> ();
			spaceObj.ApplyAir (this.m_AirValue);
			if (this.OnApplyAir != null) {
				this.OnApplyAir.Invoke ();
			}
		}
	}

	public void Reset() {
		if (this.m_Astronaut == null)
			return;
		var direction = this.m_Astronaut.transform.position - this.m_Transform.position;
		var nextTarget = direction.normalized * this.m_MinRadius + this.m_Astronaut.transform.position;
		this.m_Transform.position = new Vector3 (nextTarget.x, nextTarget.y, 0f);
		this.m_LifeTime = this.m_LifeTimeInterval;
	}

}
