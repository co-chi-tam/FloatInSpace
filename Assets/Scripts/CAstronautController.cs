using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CAstronautController : MonoBehaviour, ISpaceObject {

	[Header("Control")]
	[SerializeField]	protected Animator m_Animator;

	[Header("Data")]
	[SerializeField]	protected int m_CurrentHealth = 10;
	public int currentHealth {
		get { return this.m_CurrentHealth; }
		set { this.m_CurrentHealth = value; }
	}
	[SerializeField]	protected int m_MaxHealth = 10;
	public int maxHealth {
		get { return this.m_MaxHealth; }
		set { this.m_MaxHealth = value; }
	}
	[SerializeField]	protected int m_CurrentAir = 50;
	public int currentAir {
		get { return this.m_CurrentAir; }
		set { this.m_CurrentAir = value; }
	}
	[SerializeField]	protected int m_MaxAir = 100;
	public int maxAir {
		get { return this.m_MaxAir; }
		set { this.m_MaxAir = value; }
	}
	[SerializeField]	protected int m_AirPerTime = 1;
	[SerializeField]	protected float m_AirConsumeTime = 3f;
	[SerializeField]	protected float m_FloatSpeed = 5f;
	[SerializeField]	protected AnimationCurve m_FloatCurve;
	[SerializeField]	protected float m_FloatDamping = 0.5f;
	[SerializeField]	protected float m_TurnSpeed = 5f;
	[SerializeField]	protected float m_TurnDamping = 0.5f;

	[Header("UI")]
	[SerializeField]	protected Image m_HealthImage;
	[SerializeField]	protected Image m_AirImage;

	[Header("Event")]
	public UnityEvent OnApplyHealth;
	public UnityEvent OnApplyAir;
	public UnityEvent OnEndHealth;
	public UnityEvent OnEndAir;

	protected Transform m_Transform;
	protected float m_FloatDetail = 0f;
	protected float m_AirTime = 3f;

	protected virtual void Awake() {
		this.m_Transform = this.transform;
		CGameSetting.IsEndGame = true;
	}

	protected virtual void Update() {
		if (CGameSetting.IsEndGame)
			return;
		this.m_FloatDetail += Time.deltaTime;
		this.Floating ();
		this.Turn ();
		this.ConsumeAir ();
		this.m_FloatDetail = this.m_FloatDetail >= 1f ? 0f : this.m_FloatDetail;
	}

	protected virtual void LateUpdate() {
		this.UpdateUI ();
	}

	public void StartGame() {
		CGameSetting.IsEndGame = false;
	}

	public void Floating() {
		var value = this.m_Transform.up;
		var speed = this.m_FloatSpeed * this.m_FloatCurve.Evaluate (this.m_FloatDetail);
		var movePosition = this.m_Transform.position + (value * speed * Time.deltaTime);
		this.m_Transform.position = Vector3.Lerp (this.m_Transform.position, movePosition, this.m_FloatDamping);
	}

	public void Turn() {
		var value = this.m_Transform.rotation.eulerAngles;
		if (Input.GetMouseButton (0)) {
			var turn = Input.mousePosition.x - ((float)Screen.width / 2f);
			value.z += this.m_TurnSpeed * (turn > 0 ? -1 : 1);
		}
		this.m_Transform.rotation = Quaternion.Lerp (this.m_Transform.rotation, Quaternion.Euler (value), this.m_TurnDamping);
	}

	public void ConsumeAir() {
		if (this.m_AirTime > 0f) {
			this.m_AirTime -= Time.deltaTime;
		} else {
			this.ApplyAir (-this.m_AirPerTime);
			this.m_AirTime = this.m_AirConsumeTime;
		}
	}

	public void UpdateUI () {
		if (this.m_HealthImage == null
		    || this.m_AirImage == null)
			return;
		this.m_HealthImage.fillAmount = (float)this.m_CurrentHealth / this.m_MaxHealth;
		this.m_AirImage.fillAmount = (float)this.m_CurrentAir / this.m_MaxAir;
	}

	public void ApplyAir(int value) {
		this.m_CurrentAir += value;
		this.m_CurrentAir = this.m_CurrentAir > this.m_MaxAir ? this.m_MaxAir : this.m_CurrentAir;
		if (value > 0) {
			if (this.OnApplyAir != null) {
				this.OnApplyAir.Invoke ();
			}
		}
		if (this.m_CurrentAir <= 0) {
			CGameSetting.IsEndGame = true;
			if (this.OnEndAir != null) {
				this.OnEndAir.Invoke ();
			}
		}
	}

	public void ApplyDamage(int value) {
		this.m_CurrentHealth += value;
		this.m_CurrentHealth = this.m_CurrentHealth > this.m_MaxHealth ? this.m_MaxHealth : this.m_CurrentHealth;
		if (this.OnApplyHealth != null) {
			this.OnApplyHealth.Invoke ();
		}
		if (this.m_CurrentHealth <= 0) {
			CGameSetting.IsEndGame = true;
			if (this.OnEndHealth != null) {
				this.OnEndHealth.Invoke ();
			}
		}
	}

}
