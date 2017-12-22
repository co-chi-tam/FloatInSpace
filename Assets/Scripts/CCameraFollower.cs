using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCameraFollower : MonoBehaviour {

	[Header ("Control")]
	[SerializeField]	protected Transform m_Follower;
	[SerializeField]	protected Vector3 m_OffsetPosition = new Vector3 (0f, 0f, -10f);
	[SerializeField]	protected float m_MoveDamping = 0.5f;

	protected Transform m_Transform;

	protected void Awake() {
		this.m_Transform = this.transform;
	}

	protected void Update() {
		if (this.m_Follower == null)
			return;
		var followPosition = this.m_Follower.position + this.m_OffsetPosition;
		this.m_Transform.position = Vector3.Lerp (this.m_Transform.position, followPosition, this.m_MoveDamping);
	}

}
