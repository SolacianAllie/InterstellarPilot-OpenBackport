using System.Collections.Generic;
using DigitalRubyShared;
using OpenFrontier.IP.Engine;
using UnityEngine;
using UnityEngine.EventSystems;
using Input = OpenFrontier.LegacyInput;

namespace OpenFrontier.IP.UI
{
	public class CameraSpectator : MonoBehaviour
	{
		public enum SpectateState
		{
			None,
			Viewpoint,
			Orbit,
			Flyby,
			ManualOrbit,
			Transit
		}

		public float OrbitDistanceMultiplier = 1f;

		public float OrbitPanGestureMinRotationSpeed = 1f;

		public float OrbitPanGestureMaxRotationSpeed = 2f;

		public float OrbitPanGestureMultiplier = 10f;

		public float OrbitScaleGestureRate = 4f;

		public float OrbitScaleMouseWheelRate = 4f;

		[Tooltip("The threshold in units before zooming begins to happen. Start distance must change this much in order to start the gesture.")]
		[Range(0f, 3f)]
		public float OrbitScaleGestureZoomThresholdUnits = 0.15f;

		public GameObject OrbitScaleGesturePlatformSpecificView;

		public float MaxOrbitAngleX = 30f;

		public float MaxManualOrbitAngleX = 75f;

		public float OrbitRotationRateMultiplierX = 0.4f;

		public float OrbitStationDistanceMultiplier = 2f;

		public bool AllowFlyby = true;

		public bool AllowOrbit = true;

		public bool AllowViewport = true;

		private bool alternateState = true;

		public bool UseRealTime = true;

		private List<SpectateState> availableStates = new List<SpectateState>();

		private Vector3 cameraAngle = Vector3.zero;

		private Vector3 currentRotRate = Vector3.zero;

		private SpectateState currentState;

		private int currentViewportIndex;

		public float FlyByMaxDist = 300f;

		public float FlyByMinDist;

		public float FlyByMinSpdAllowed = 4f;

		public float FlyByRadiusMultiplier = 1f;

		public float FlyBySpdMultiplier = 3f;

		public float MaxFlyByAngle = 20f;

		[SerializeField]
		private float minOrbitDistance = 40f;

		private double nextStateChangeTime;

		public float OrbitRadiusDistanceFudge = 2f;

		[SerializeField]
		private float minOrbitRadiusMultiplier = 1.5f;

		[SerializeField]
		private float maxOrbitRadiusMultiplier = 2.4f;

		[SerializeField]
		private float minManualOrbitRadiusMultiplier = 1f;

		[SerializeField]
		private float maxManualOrbitRadiusMultiplier = 3f;

		[SerializeField]
		public float OrbitSpeed = 4f;

		public float StateDuration = 5f;

		[SerializeField]
		private GameObject target;

		private Unit targetUnit;

		private Unit lastTargetRootUnit;

		// Open Frontier: when the spectate TARGET changes, fly to the new
		// subject instead of teleporting to it. The camera SmoothDamps
		// from wherever it is to an approach anchor near the new target
		// while looking at it, then settles into a normal orbit. Player
		// pan/zoom gestures still seize control mid-flight (they enter
		// ManualOrbit, cancelling the transit).
		public bool SmoothTargetTransitions = true;

		public float TransitSmoothTime = 1.1f;

		public float TransitMinSpeed = 800f;

		public float TransitMaxSpeed = 12000f;

		[Tooltip("Vertical arc height as a fraction of the transit distance (clamped 600-6000u).")]
		public float TransitLiftFactor = 0.18f;

		[Tooltip("Path noise amplitude as a fraction of the transit distance (clamped 100-1500u); fades out on final approach.")]
		public float TransitNoiseFactor = 0.1f;

		[Tooltip("How fast the camera turns toward its subject in transit (deg/sec).")]
		public float TransitRotationSpeed = 75f;

		[Tooltip("Bank angle per unit of lateral acceleration while turning in transit.")]
		public float TransitBankFactor = 0.015f;

		private float transitStartTime;

		private Vector3 transitVelocity;

		private Vector3 transitOffsetDirection;

		private float transitMaxSpeedCurrent;

		private float transitInitialDistance;

		private float transitLiftSign;

		private bool skipOrbitEntryRandomization;

		private double lastStateChangeTime;

		private Vector3 lastTargetPosition = Vector3.zero;

		public float CurrentOrbitDistance;

		public ScaleGestureRecognizer OrbitScaleGesture { get; private set; }

		public PanGestureRecognizer OrbitPanGesture { get; private set; }

		public double LastStateChangeTime => lastStateChangeTime;

		public SpectateState CurrentState
		{
			get
			{
				return currentState;
			}
			private set
			{
				if (currentState != value)
				{
					lastStateChangeTime = CurrentGameTime();
				}
				currentState = value;
				if (target != null)
				{
					switch (currentState)
					{
					case SpectateState.Flyby:
					{
						Vector3 euler = new Vector3((0f - MaxFlyByAngle) / 2f + Random.value * MaxFlyByAngle, (0f - MaxFlyByAngle) / 2f + Random.value * MaxFlyByAngle, 0f);
						float num = Mathf.Clamp(TargetSpeed * FlyBySpdMultiplier, FlyByMinDist, FlyByMaxDist);
						Vector3 vector = Quaternion.Euler(GetOrbitCameraAngle()) * Vector3.forward * FlyByRadiusMultiplier * TargetRadius;
						transform.position = target.transform.TransformPoint(vector + Quaternion.Euler(euler) * Vector3.forward * num);
						break;
					}
					case SpectateState.Orbit:
						// After a transit the camera is ALREADY at the
						// orbit point with matching angle+distance -
						// randomizing here would snap at the dock.
						if (!skipOrbitEntryRandomization)
						{
							cameraAngle = GetOrbitCameraAngle();
							SetRandomOrbitDistance();
						}
						skipOrbitEntryRandomization = false;
						currentRotRate = new Vector3(0f - OrbitSpeed + Random.value * OrbitSpeed * 2f * OrbitRotationRateMultiplierX, 0f - OrbitSpeed + Random.value * OrbitSpeed * 2f, 0f);
						break;
					}
				}
				ApplyState();
			}
		}

		public float TargetSpeed => Vector3.Distance(lastTargetPosition, target.transform.position);

		public float TargetRadius
		{
			get
			{
				if (targetUnit != null)
				{
					return targetUnit.GetRootUnit().UnitClass.ShieldRingRadius;
				}
				return 0f;
			}
		}

		public bool AlternateViewState
		{
			get
			{
				return alternateState;
			}
			set
			{
				alternateState = value;
			}
		}

		public int CurrentViewportIndex
		{
			get
			{
				return currentViewportIndex;
			}
			set
			{
				currentViewportIndex = value;
			}
		}

		public float RotRate
		{
			get
			{
				return OrbitSpeed;
			}
			set
			{
				OrbitSpeed = value;
			}
		}

		public Unit TargetRootUnit
		{
			get
			{
				if (targetUnit != null)
				{
					return targetUnit.GetRootUnit();
				}
				return null;
			}
		}

		public GameObject Target
		{
			get
			{
				return target;
			}
			set
			{
				if (target != value)
				{
					bool hadValidPreviousTarget = target != null && currentState != SpectateState.None;
					GameObject previousTarget = target;
					target = value;
					if (target != null)
					{
						targetUnit = target.GetComponent<Unit>();
						if (SmoothTargetTransitions && hadValidPreviousTarget && targetUnit != null)
						{
							BeginTransit(previousTarget);
						}
						else
						{
							ChooseState();
						}
						ApplyState();
						CameraMoved();
						UpdateLastTargetPosition();
					}
					else
					{
						targetUnit = null;
						CurrentState = SpectateState.None;
					}
				}
			}
		}

		public float MinOrbitDistance
		{
			get
			{
				return minOrbitDistance;
			}
			set
			{
				minOrbitDistance = value;
			}
		}

		public float MaxOrbitRadiusMultiplier
		{
			get
			{
				return maxOrbitRadiusMultiplier;
			}
			set
			{
				maxOrbitRadiusMultiplier = value;
			}
		}

		public float MinOrbitRadiusMultiplier
		{
			get
			{
				return minOrbitRadiusMultiplier;
			}
			set
			{
				minOrbitRadiusMultiplier = value;
			}
		}

		public EngineASX Engine => EngineASX.Instance;

		public void SetRandomOrbitDistance()
		{
			OrbitDistanceMultiplier = Random.Range(minOrbitRadiusMultiplier, maxOrbitRadiusMultiplier);
			SetOrbitDistance();
		}

		private void SetOrbitDistance()
		{
			CurrentOrbitDistance = CalculateOrbitDistance();
		}

		private double CurrentGameTime()
		{
			if (!UseRealTime)
			{
				return Time.timeAsDouble;
			}
			return Time.realtimeSinceStartupAsDouble;
		}

		private void CameraMoved()
		{
			Engine.OnCameraMoved();
		}

		private void OnEnable()
		{
			OrbitScaleGesture = new ScaleGestureRecognizer();
			OrbitScaleGesture.StateUpdated += ScaleGesture_Updated;
			OrbitScaleGesture.ThresholdUnits = OrbitScaleGestureZoomThresholdUnits;
			OrbitScaleGesture.PlatformSpecificView = OrbitScaleGesturePlatformSpecificView;
			FingersScript.Instance.AddGesture(OrbitScaleGesture);
			OrbitPanGesture = new PanGestureRecognizer();
			OrbitPanGesture.StateUpdated += PanGestureUpdated;
			OrbitPanGesture.PlatformSpecificView = OrbitScaleGesturePlatformSpecificView;
			FingersScript.Instance.AddGesture(OrbitPanGesture);
		}

		private void OnDisable()
		{
			if (FingersScript.HasInstance)
			{
				FingersScript.Instance.RemoveGesture(OrbitPanGesture);
				FingersScript.Instance.RemoveGesture(OrbitScaleGesture);
			}
		}

		private void PanGestureUpdated(GestureRecognizer r)
		{
			if (r.State == GestureRecognizerState.Began)
			{
				CurrentState = SpectateState.ManualOrbit;
			}
			if (r.State == GestureRecognizerState.Executing)
			{
				float num = Mathf.Lerp(OrbitPanGestureMinRotationSpeed, OrbitPanGestureMaxRotationSpeed, GameController.Instance.CameraDragRotateSensitivity) * OrbitPanGestureMultiplier;
				num *= GameController.Instance.GetCameraDragRotateSensitivityMultiplier();
				float velocityX = r.VelocityX;
				float velocityY = r.VelocityY;
				velocityX *= RealTime.deltaTime;
				velocityY *= RealTime.deltaTime;
				cameraAngle.y += DeviceInfo.PixelsToUnits(velocityX) * num;
				cameraAngle.x = Mathf.Clamp(cameraAngle.x + DeviceInfo.PixelsToUnits(velocityY) * num, 0f - MaxManualOrbitAngleX, MaxManualOrbitAngleX);
			}
		}

		private void ScaleGesture_Updated(GestureRecognizer gesture)
		{
			if (gesture.State == GestureRecognizerState.Began)
			{
				CurrentState = SpectateState.ManualOrbit;
			}
			if (gesture.State == GestureRecognizerState.Executing)
			{
				OrbitDistanceMultiplier = Mathf.Clamp(OrbitDistanceMultiplier - OrbitScaleGesture.ScaleDistanceDeltaY * OrbitScaleGestureRate, minManualOrbitRadiusMultiplier, maxManualOrbitRadiusMultiplier);
				SetOrbitDistance();
			}
		}

		private void Update()
		{
			if (!(target != null))
			{
				return;
			}
			if (currentState != SpectateState.ManualOrbit && currentState != SpectateState.Transit)
			{
				if (CurrentGameTime() > nextStateChangeTime)
				{
					ChooseState();
				}
				else if (targetUnit != null && TargetRootUnit != lastTargetRootUnit)
				{
					SetRandomOrbitDistance();
				}
			}
			else
			{
				MouseWheelScale();
				if (targetUnit != null && TargetRootUnit != lastTargetRootUnit)
				{
					SetOrbitDistance();
				}
			}
			ApplyState();
			UpdateLastTargetPosition();
			lastTargetRootUnit = TargetRootUnit;
		}

		private void MouseWheelScale()
		{
			if (Input.mouseScrollDelta.y != 0f && (!(EventSystem.current != null) || !EventSystem.current.IsPointerOverGameObject()))
			{
				OrbitDistanceMultiplier = Mathf.Clamp(OrbitDistanceMultiplier - Input.mouseScrollDelta.y * OrbitScaleMouseWheelRate, minManualOrbitRadiusMultiplier, maxManualOrbitRadiusMultiplier);
				SetOrbitDistance();
			}
		}

		private void UpdateLastTargetPosition()
		{
			lastTargetPosition = target.transform.position;
		}

		// Smooth transition entry: pick a fixed approach direction at
		// orbit distance from the new target; the anchor tracks the
		// (moving) ship while SmoothDamp flies us to it. The camera
		// inherits the OLD subject's velocity (momentum breakaway),
		// lifts off the world plane in a sine arc with gentle noise,
		// then matches the new ship's motion on final approach. Speed
		// scales with distance so rim-to-core jumps take seconds.
		private void BeginTransit(GameObject previousTarget)
		{
			CurrentOrbitDistance = CalculateOrbitDistance();
			transitOffsetDirection = Quaternion.Euler(GetOrbitCameraAngle()) * Vector3.forward;
			transitInitialDistance = Mathf.Max(Vector3.Distance(transform.position, GetTargetRootPosition()), 1f);
			transitMaxSpeedCurrent = Mathf.Clamp(transitInitialDistance / 2f, TransitMinSpeed, TransitMaxSpeed);
			transitLiftSign = ((Random.value < 0.5f) ? (-1f) : 1f);
			transitVelocity = Vector3.zero;
			if (previousTarget != null)
			{
				transitVelocity = Vector3.ClampMagnitude((previousTarget.transform.position - lastTargetPosition) / Mathf.Max(Time.deltaTime, 0.001f), 3000f);
			}
			transitStartTime = Time.time;
			CurrentState = SpectateState.Transit;
		}

		private void ApplyState()
		{
			switch (currentState)
			{
			case SpectateState.Transit:
			{
				float realDeltaTime = (float)GameController.Instance.RealDeltaTime;
				Vector3 targetRootPosition = GetTargetRootPosition();
				Vector3 vector2 = targetRootPosition + transitOffsetDirection * CurrentOrbitDistance;
				float num = Vector3.Distance(transform.position, vector2);
				float num2 = 1f - Mathf.Clamp01(num / transitInitialDistance);
				// Sine arc off the world plane + perlin wander, fading
				// out on final approach.
				float num3 = Mathf.Clamp(transitInitialDistance * TransitLiftFactor, 600f, 6000f) * transitLiftSign * Mathf.Sin(Mathf.PI * Mathf.Min(num2 * 1.2f, 1f));
				float num4 = Time.time * 0.35f;
				float num5 = Mathf.Clamp(transitInitialDistance * TransitNoiseFactor, 100f, 1500f) * (1f - num2);
				Vector3 vector3 = vector2 + Vector3.up * num3 + new Vector3(Mathf.PerlinNoise(num4, 0.3f) - 0.5f, (Mathf.PerlinNoise(num4, 7.7f) - 0.5f) * 0.6f, Mathf.PerlinNoise(3.1f, num4) - 0.5f) * num5;
				// Ship-like flight: the velocity is STEERED, not damped -
				// thrust-limited acceleration toward a desired velocity
				// that cruises by distance, slows on arrival, and matches
				// the target ship's own motion for the dock.
				Vector3 vector4 = ((targetUnit != null && targetUnit.GetRootUnit().RBody != null) ? targetUnit.GetRootUnit().RBody.velocity : Vector3.zero);
				float num6 = Mathf.Min(transitMaxSpeedCurrent, num * 1.5f + 30f);
				Vector3 vector7 = vector4 + (vector3 - transform.position).normalized * num6;
				float num8 = Mathf.Clamp(transitMaxSpeedCurrent * 0.6f, 400f, 6000f);
				Vector3 vector9 = transitVelocity;
				transitVelocity = Vector3.MoveTowards(transitVelocity, vector7, num8 * realDeltaTime);
				transform.position += transitVelocity * realDeltaTime;
				// Smooth turn toward the subject (turn-rate limited),
				// banking into the turn like a ship.
				Quaternion quaternion = Quaternion.LookRotation(targetRootPosition - transform.position);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, TransitRotationSpeed * realDeltaTime);
				float num10 = Mathf.Clamp(Vector3.Dot(transform.right, (transitVelocity - vector9) / Mathf.Max(realDeltaTime, 0.001f)) * TransitBankFactor, -30f, 30f);
				transform.Rotate(Vector3.forward, 0f - num10, Space.Self);
				// Dock when close AND velocity-matched (timeout fallback
				// for a target that keeps outrunning the camera).
				bool flag = num < Mathf.Max(25f, CurrentOrbitDistance * 0.1f) && (transitVelocity - vector4).magnitude < Mathf.Max(40f, vector4.magnitude * 0.5f);
				bool flag2 = Time.time > transitStartTime + transitInitialDistance / TransitMinSpeed + 10f;
				if (flag || flag2)
				{
					// Hand Orbit the ACTUAL arrival angle and distance
					// (and suppress its entry randomization) so the
					// camera continues from exactly where the flight
					// ended - no snap at the dock.
					skipOrbitEntryRandomization = true;
					Vector3 vector10 = transform.position - targetRootPosition;
					CurrentOrbitDistance = vector10.magnitude;
					Vector3 euler = Quaternion.LookRotation(vector10.normalized).eulerAngles;
					cameraAngle = new Vector3(euler.x, euler.y, 0f);
					CurrentState = SpectateState.Orbit;
					SetNextStateChangeTime();
					CameraMoved();
				}
				break;
			}
			case SpectateState.ManualOrbit:
				ApplyOrbit();
				break;
			case SpectateState.Orbit:
				ApplyOrbit();
				cameraAngle += currentRotRate * (float)GameController.Instance.RealDeltaTime;
				break;
			case SpectateState.Flyby:
				if (TargetSpeed == 0f)
				{
					CurrentState = SpectateState.Orbit;
					CameraMoved();
				}
				else
				{
					gameObject.transform.LookAt(target.transform);
				}
				break;
			}
		}

		private void ApplyOrbit()
		{
			Quaternion quaternion = Quaternion.Euler(cameraAngle);
			Vector3 vector = Vector3.forward * CurrentOrbitDistance;
			transform.position = GetTargetRootPosition() + quaternion * vector;
			gameObject.transform.LookAt(GetTargetRootPosition());
		}

		private Vector3 GetTargetRootPosition()
		{
			if (targetUnit != null)
			{
				return TargetRootUnit.transform.position;
			}
			return target.transform.position;
		}

		private float CalculateOrbitDistance()
		{
			float num = OrbitRadiusDistanceFudge + TargetRadius * OrbitDistanceMultiplier;
			if (targetUnit != null)
			{
				Unit rootUnit = targetUnit.GetRootUnit();
				if (rootUnit.UnitType == UnitType.Station)
				{
					num *= OrbitStationDistanceMultiplier;
				}
				if (rootUnit.UnitClass.DisplayData != null)
				{
					num *= rootUnit.UnitClass.DisplayData.CameraOrbitDistanceMultipler;
				}
			}
			return Mathf.Max(minOrbitDistance, num);
		}

		public void ChooseState()
		{
			PopulateAvailableStates();
			if (availableStates.Count > 0)
			{
				CurrentState = availableStates[Random.Range(0, availableStates.Count)];
			}
			SetNextStateChangeTime();
		}

		private void SetNextStateChangeTime()
		{
			nextStateChangeTime = CurrentGameTime() + (double)StateDuration;
		}

		private Vector3 GetOrbitCameraAngle()
		{
			return new Vector3(0f - MaxOrbitAngleX + Random.value * MaxOrbitAngleX * 2f, Random.value * 360f, 0f);
		}

		private void PopulateAvailableStates()
		{
			availableStates.Clear();
			if (target != null)
			{
				if (AllowFlyby && TargetSpeed > FlyByMinSpdAllowed)
				{
					availableStates.Add(SpectateState.Flyby);
				}
				if (AllowViewport && targetUnit != null)
				{
					availableStates.Add(SpectateState.Viewpoint);
				}
				if (AllowOrbit)
				{
					availableStates.Add(SpectateState.Orbit);
				}
			}
		}
	}
}
