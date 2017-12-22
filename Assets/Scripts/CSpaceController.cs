using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSpaceController : MonoBehaviour {

	[Header("Planets")]
	[SerializeField]	protected float m_PlanetDistance = 20f;
	[SerializeField]	protected string m_PlanetNamePattern = "x{0}+y{1}";
	protected Vector2[] m_PlanetPatterns = new Vector2[] {
		new Vector2 (-1f, 1f), new Vector2 (0f, 1f), new Vector2 (1f, 1f), 
		new Vector2 (-1f, 0f), new Vector2 (0f, 0f), new Vector2 (1f, 0f), 
		new Vector2 (-1f, -1f), new Vector2 (0f, -1f), new Vector2 (1f, -1f), 
	};
	[SerializeField]	protected List<Transform> m_UsedPlanets;
	protected Queue<Transform> m_ReusePlanets;

	[Header("Astronaut")]
	[SerializeField]	protected Transform m_Astronaut;
	[SerializeField]	protected Vector2 m_CurrentPosition;
	protected Vector2 m_PreviousPosition = Vector2.zero;
	protected bool m_NeedUpdate = false;

	protected void Awake() {
		this.m_ReusePlanets = new Queue<Transform> ();
	}

	protected void Start() {
		this.InitPlanet();
	}

	protected void LateUpdate() {
		this.CheckCurrentPosition ();
		this.CheckPlanetPatterns ();
		if (this.m_NeedUpdate) {
			this.UpdateReusePlanets ();
			this.UpdatePlanets ();
		}
	}

	protected void InitPlanet() {
		var childCount = this.transform.childCount;
		for (int i = 0; i < childCount; i++) {
			var child = this.transform.GetChild (i);
			var updatePos = this.m_PlanetPatterns [i];
			this.UpdatePlanetPosition (child, updatePos); 
			this.m_UsedPlanets.Add (child);
		}
	}

	protected void CheckCurrentPosition() {
		if (this.m_Astronaut == null)
			return;
		var position = this.m_Astronaut.position;
		this.m_CurrentPosition.x = Mathf.RoundToInt (position.x / this.m_PlanetDistance);
		this.m_CurrentPosition.y = Mathf.RoundToInt (position.y / this.m_PlanetDistance);
		if (this.m_CurrentPosition.x != this.m_PreviousPosition.x
		    || this.m_CurrentPosition.y != this.m_PreviousPosition.y) {
			this.m_NeedUpdate = true;
			this.m_PreviousPosition.x = this.m_CurrentPosition.x;
			this.m_PreviousPosition.y = this.m_CurrentPosition.y;
		} else {
			this.m_NeedUpdate = false;
		}
	}

	protected void CheckPlanetPatterns() {
		var currentPos 	= this.m_CurrentPosition;
		// topLeft
		this.m_PlanetPatterns[0].x = currentPos.x - 1f;
		this.m_PlanetPatterns[0].y = currentPos.y + 1f;
		// topUp
		this.m_PlanetPatterns[1].x = currentPos.x;
		this.m_PlanetPatterns[1].y = currentPos.y + 1f; 
		// topRight
		this.m_PlanetPatterns[2].x = currentPos.x + 1f;
		this.m_PlanetPatterns[2].y = currentPos.y + 1f;
		// left
		this.m_PlanetPatterns[3].x = currentPos.x - 1f;
		this.m_PlanetPatterns[3].y = currentPos.y;
		// center
		this.m_PlanetPatterns[4].x = currentPos.x;
		this.m_PlanetPatterns[4].y = currentPos.y; 
		// right
		this.m_PlanetPatterns[5].x = currentPos.x + 1f; 
		this.m_PlanetPatterns[5].y = currentPos.y;
		// bottomLeft
		this.m_PlanetPatterns[6].x = currentPos.x - 1f;
		this.m_PlanetPatterns[6].y = currentPos.y - 1f; 
		// bottomDown
		this.m_PlanetPatterns[7].x = currentPos.x;
		this.m_PlanetPatterns[7].y = currentPos.y - 1f; 
		// bottomRight
		this.m_PlanetPatterns[8].x = currentPos.x + 1f;
		this.m_PlanetPatterns[8].y = currentPos.y - 1f;
	}

	protected void UpdateReusePlanets() {
		for (int x = 0; x < this.m_UsedPlanets.Count; x++) {
			var checkName = this.m_UsedPlanets [x].name;
			var isGoodPlanet = false;
			for (int i = 0; i < this.m_PlanetPatterns.Length; i++) {
				var planetPos = this.m_PlanetPatterns[i];
				var planetName = string.Format (this.m_PlanetNamePattern, planetPos.x, planetPos.y);
				if (planetName == checkName) {
					isGoodPlanet = true;
					break;
				}
			}
			if (isGoodPlanet == false) {
				if (this.m_ReusePlanets.Contains (this.m_UsedPlanets [x]) == false) {
					this.m_ReusePlanets.Enqueue (this.m_UsedPlanets [x]);
				}
			}
		}
	}

	protected void UpdatePlanets() {
		for (int i = 0; i < this.m_PlanetPatterns.Length; i++) {
			var planetPos = this.m_PlanetPatterns[i];
			var planetName = string.Format (this.m_PlanetNamePattern, planetPos.x, planetPos.y);
			var isGoodPlanet = false;
			for (int x = 0; x < this.m_UsedPlanets.Count; x++) {
				var checkName = this.m_UsedPlanets [x].name;
				isGoodPlanet |= planetName == checkName;
			}
			if (isGoodPlanet == false) {
				var outSidePlane = this.m_ReusePlanets.Dequeue ();
				this.UpdatePlanetPosition (outSidePlane, planetPos);
			}
		}
	}

	protected void UpdatePlanetPosition (Transform planet, Vector2 pos) {
		planet.name = string.Format (this.m_PlanetNamePattern, pos.x, pos.y);
		planet.position = new Vector3 (pos.x * this.m_PlanetDistance, pos.y * this.m_PlanetDistance, 0f);
	}

}
