using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Autohand
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[DefaultExecutionOrder(-3)]
	[HelpURL("https://earnestrobot.notion.site/Auto-Move-Player-02d91305a4294e039049bd45cacc5b90")]
	public class AutoHandPlayer : MonoBehaviour
	{
		public const string HandPlayerLayer = "HandPlayer";
		private const int groundRayCount = 21;

		private static bool notFound;
		public static AutoHandPlayer _Instance;


		[AutoHeader("Auto Hand Player")] public bool ignoreMe;

		[Tooltip("The tracked headCamera object")]
		public Camera headCamera;

		[Tooltip(
			"The object that represents the forward direction movement, usually should be set as the camera or a tracked controller")]
		public Transform forwardFollow;

		[Tooltip(
			"This should NOT be a child of this body. This should be a GameObject that contains all the tracked objects (head/controllers)")]
		public Transform trackingContainer;

		public Hand handRight;
		public Hand handLeft;

		[AutoToggleHeader("Movement")] public bool useMovement = true;

		[EnableIf("useMovement")] [FormerlySerializedAs("moveSpeed")] [Tooltip("Movement speed when isGrounded")]
		public float maxMoveSpeed = 1.5f;

		[EnableIf("useMovement")] [Tooltip("Movement acceleration when isGrounded")]
		public float moveAcceleration = 10f;

		[EnableIf("useMovement")] [Tooltip("Movement acceleration when isGrounded")]
		public float groundedDrag = 4f;

		[EnableIf("useMovement")] public float heightSmoothSpeed = 20f;

		[AutoToggleHeader("Snap Turning")] [Tooltip("Whether or not to use snap turning or smooth turning")] [Min(0)]
		public bool snapTurning = true;

		[Tooltip("turn speed when not using snap turning - if snap turning, represents angle per snap")]
		[ShowIf("snapTurning")]
		public float snapTurnAngle = 30f;

		[HideIf("snapTurning")] public float smoothTurnSpeed = 10f;

		[AutoToggleHeader("Height")] public bool ignoreMe2 = true;

		[ShowIf("ignoreMe2")] public float heightOffset;

		[ShowIf("ignoreMe2")] public bool crouching;

		[ShowIf("ignoreMe2")] public float crouchHeight = 0.6f;

		[ShowIf("ignoreMe2")]
		[Tooltip("Whether or not the capsule height should be adjusted to match the headCamera height")]
		public bool autoAdjustColliderHeight = true;

		[ShowIf("ignoreMe2")]
		[Tooltip(
			"Minimum and maximum auto adjusted height, to adjust height without auto adjustment change capsule collider height instead")]
		public Vector2 minMaxHeight = new(0.5f, 2.5f);

		[ShowIf("ignoreMe2")] public bool useHeadCollision = true;

		[ShowIf("ignoreMe2")] public float headRadius = 0.15f;


		[AutoToggleHeader("Use Grounding")] public bool useGrounding = true;

		[EnableIf("useGrounding")] [Tooltip("Maximum height that the body can step up onto")] [Min(0)]
		public float maxStepHeight = 0.3f;

		[EnableIf("useGrounding")] [Tooltip("Maximum angle the player can walk on")] [Min(0)]
		public float maxStepAngle = 30f;

		[EnableIf("useGrounding")] [Tooltip("The layers that count as ground")]
		public LayerMask groundLayerMask;

		[AutoToggleHeader("Enable Climbing")]
		[Tooltip("Whether or not the player can use Climbable objects  (Objects with the Climbable component)")]
		public bool allowClimbing = true;

		[Tooltip("Whether or not the player move while climbing")] [ShowIf("allowClimbing")]
		public bool allowClimbingMovement = true;

		[Tooltip("How quickly the player can climb")] [ShowIf("allowClimbing")]
		public Vector3 climbingStrength = new(20f, 20f, 20f);

		public float climbingAcceleration = 30f;
		public float climbingDrag = 5f;

		[AutoToggleHeader("Enable Pushing")]
		[Tooltip("Whether or not the player can use Pushable objects (Objects with the Pushable component)")]
		public bool allowBodyPushing = true;

		[Tooltip("How quickly the player can climb")] [EnableIf("allowBodyPushing")]
		public Vector3 pushingStrength = new(10f, 10f, 10f);

		public float pushingAcceleration = 10f;
		public float pushingDrag = 3f;

		[AutoToggleHeader("Enable Platforming")]
		[Tooltip(
			"Platforms will move the player with them. A platform is an object with the Transform component on it")]
		public bool allowPlatforms = true;

		private bool axisReset = true;
		private CapsuleCollider bodyCapsule;

		private Vector3 climbAxis;
		private readonly Dictionary<Hand, Climbable> climbing = new();
		private RaycastHit closestHit;

		private Coroutine disableGroundingRoutine;
		private readonly float groundedOffset = 0.05f;
		private int handPlayerMask;

		private HeadPhysicsFollower headPhysicsFollower;
		private float highestPoint;
		private bool ignoreIterpolationFrame;
		private bool isGrounded;
		private bool lastCrouching;
		private float lastCrouchingHeight;
		private Vector3 lastHeadPos;
		private Hand lastLeftHand;

		private Vector3 lastPlatformPosition;
		private Quaternion lastPlatformRotation;

		private Hand lastRightHand;
		private Vector3 lastUpdatePosition;
		private float lastUpdateTime;
		private Vector3 moveDirection;

		[AutoToggleHeader("Use Input Deadzone")]
		private readonly float movementDeadzone = 0.1f;


		private RaycastHit newClosestHit;


		private Vector3 offset;
		private float playerHeight;
		private Vector3 pushAxis;
		private readonly Dictionary<Pushable, Hand> pushLeft = new();
		private readonly Dictionary<Pushable, int> pushLeftCount = new();
		private readonly Dictionary<Pushable, Hand> pushRight = new();
		private readonly Dictionary<Pushable, int> pushRightCount = new();
		private Quaternion startRot;
		private Vector3 targetPosOffset;
		private Vector3 targetTrackedPos;

		private bool trackingStarted;
		private readonly float turnDeadzone = 0.4f;
		private float turningAxis;


		private readonly float turnResetzone = 0.3f;

		public static AutoHandPlayer Instance
		{
			get
			{
				if (_Instance == null && !notFound)
					_Instance = FindObjectOfType<AutoHandPlayer>();

				if (_Instance == null)
					notFound = true;

				return _Instance;
			}
		}

		public Rigidbody body { get; private set; }

		public virtual void Start()
		{
			startRot = headCamera.transform.rotation;
			lastUpdatePosition = transform.position;

			gameObject.layer = LayerMask.NameToLayer(HandPlayerLayer);

			bodyCapsule = GetComponent<CapsuleCollider>();

			body = GetComponent<Rigidbody>();
			body.interpolation = RigidbodyInterpolation.None;
			body.freezeRotation = true;
			if (body.collisionDetectionMode == CollisionDetectionMode.Discrete)
				body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

			if (forwardFollow == null)
				forwardFollow = headCamera.transform;


			targetTrackedPos = trackingContainer.position;
			if (useHeadCollision)
				CreateHeadFollower();
			StartCoroutine(CheckForTrackingStart());

			handPlayerMask = AutoHandExtensions.GetPhysicsLayerMask(gameObject.layer);
		}

		private void Update()
		{
			if (useMovement)
			{
				InterpolateMovement();
				UpdatePlatform(false);
				UpdateTurn(Time.deltaTime);
			}
		}

		protected virtual void FixedUpdate()
		{
			CheckHands();
			UpdatePlayerHeight();

			if (useMovement)
			{
				ApplyPushingForce();
				ApplyClimbingForce();
				Ground();
				UpdatePlatform(true);
				UpdateRigidbody(moveDirection);
				UpdateTurn(Time.fixedDeltaTime);
			}
		}

		protected virtual void OnEnable()
		{
			EnableHand(handRight);
			EnableHand(handLeft);
		}

		protected virtual void OnDisable()
		{
			DisableHand(handRight);
			DisableHand(handLeft);
		}

		private IEnumerator CheckForTrackingStart()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForFixedUpdate();
			lastHeadPos = headCamera.transform.position;
			while (!trackingStarted)
			{
				if (headCamera.transform.position != lastHeadPos)
				{
					OnHeadTrackingStarted();
					trackingStarted = true;
				}

				lastHeadPos = headCamera.transform.position;
				yield return new WaitForEndOfFrame();
			}
		}

		protected virtual void OnHeadTrackingStarted()
		{
			SetPosition(transform.position);
		}

		private void CreateHeadFollower()
		{
			if (headPhysicsFollower == null)
			{
				var headFollower = new GameObject().transform;
				headFollower.transform.position = headCamera.transform.position;
				headFollower.name = "Head Follower";
				headFollower.parent = transform.parent;

				var col = headFollower.gameObject.AddComponent<SphereCollider>();
				col.material = bodyCapsule.material;
				col.radius = bodyCapsule.radius;

				var headBody = headFollower.gameObject.AddComponent<Rigidbody>();
				headBody.drag = 5;
				headBody.angularDrag = 5;
				headBody.freezeRotation = false;
				headBody.mass = body.mass / 3f;

				headPhysicsFollower = headFollower.gameObject.AddComponent<HeadPhysicsFollower>();
				headPhysicsFollower.headCamera = headCamera;
				headPhysicsFollower.followBody = transform;
				headPhysicsFollower.trackingContainer = trackingContainer;
				//headPhysicsFollower.maxBodyDistance = maxHeadDistance;
			}
		}


		private void CheckHands()
		{
			if (lastLeftHand != handLeft)
			{
				EnableHand(handLeft);
				lastLeftHand = handLeft;
			}

			if (lastRightHand != handRight)
			{
				EnableHand(handRight);
				lastRightHand = handRight;
			}
		}


		private void EnableHand(Hand hand)
		{
			hand.OnGrabbed += OnHandGrab;
			hand.OnReleased += OnHandRelease;


			if (allowClimbing)
			{
				hand.OnGrabbed += StartClimb;
				hand.OnReleased += EndClimb;
			}

			if (allowBodyPushing)
			{
				hand.OnGrabbed += StartGrabPush;
				hand.OnReleased += EndGrabPush;
				hand.OnHandCollisionStart += StartPush;
				hand.OnHandCollisionStop += StopPush;
			}
		}

		private void DisableHand(Hand hand)
		{
			hand.OnGrabbed -= OnHandGrab;
			hand.OnReleased -= OnHandRelease;

			if (allowClimbing)
			{
				hand.OnGrabbed -= StartClimb;
				hand.OnReleased -= EndClimb;
				if (climbing.ContainsKey(hand))
					climbing.Remove(hand);
			}

			if (allowBodyPushing)
			{
				hand.OnGrabbed -= StartGrabPush;
				hand.OnReleased -= EndGrabPush;
				hand.OnHandCollisionStart -= StartPush;
				hand.OnHandCollisionStop -= StopPush;
				if (hand.left)
				{
					pushLeft.Clear();
					pushLeftCount.Clear();
				}
				else
				{
					pushRight.Clear();
					pushRightCount.Clear();
				}
			}
		}

		private void OnHandGrab(Hand hand, Grabbable grab)
		{
			grab.IgnoreColliders(bodyCapsule);
			if (headPhysicsFollower != null)
				grab?.IgnoreColliders(headPhysicsFollower.headCollider);
		}

		private void OnHandRelease(Hand hand, Grabbable grab)
		{
			grab?.IgnoreColliders(bodyCapsule, false);
			if (headPhysicsFollower != null)
				grab?.IgnoreColliders(headPhysicsFollower.headCollider, false);

			if (grab && grab.parentOnGrab)
				grab.body.velocity += body.velocity / 2f;
		}


		/// <summary>Sets move direction for this fixedupdate</summary>
		public virtual void Move(Vector2 axis, bool useDeadzone = true, bool useRelativeDirection = false)
		{
			moveDirection.x = !useDeadzone || Mathf.Abs(axis.x) > movementDeadzone ? axis.x : 0;
			moveDirection.z = !useDeadzone || Mathf.Abs(axis.y) > movementDeadzone ? axis.y : 0;
			if (useRelativeDirection)
				moveDirection = transform.rotation * moveDirection;
		}

		public virtual void Turn(float turnAxis)
		{
			turnAxis = Mathf.Abs(turnAxis) > turnDeadzone ? turnAxis : 0;
			turningAxis = turnAxis;
		}


		protected virtual void UpdateRigidbody(Vector3 moveDir)
		{
			var move = AlterDirection(moveDir);
			var yVel = body.velocity.y;

			//1. Moves velocity towards desired push direction
			if (pushAxis != Vector3.zero)
			{
				body.velocity = Vector3.MoveTowards(body.velocity, pushAxis, pushingAcceleration * Time.fixedDeltaTime);
				body.velocity = Vector3.MoveTowards(body.velocity, Vector3.zero, pushingDrag * Time.fixedDeltaTime);
			}

			//2. Moves velocity towards desired climb direction
			if (climbAxis != Vector3.zero)
			{
				body.velocity =
					Vector3.MoveTowards(body.velocity, climbAxis, climbingAcceleration * Time.fixedDeltaTime);
				body.velocity = Vector3.MoveTowards(body.velocity, Vector3.zero, climbingDrag * Time.fixedDeltaTime);
			}

			//3. Moves velocity towards desired movement direction
			if (move != Vector3.zero && CanInputMove())
				body.velocity = Vector3.MoveTowards(body.velocity, move * maxMoveSpeed,
					moveAcceleration * Time.fixedDeltaTime);

			//4. This creates extra drag when grounded to simulate foot strength, or if flying greats drag in every direction when not moving
			if (isGrounded && move.magnitude <= movementDeadzone)
				body.velocity = Vector3.MoveTowards(body.velocity,
					transform.rotation * new Vector3(0, body.velocity.y, 0), groundedDrag * Time.fixedDeltaTime);
			else if (!useGrounding && move.magnitude < 0.2f)
				body.velocity = Vector3.MoveTowards(body.velocity, transform.rotation * Vector3.zero,
					groundedDrag * Time.fixedDeltaTime);

			//5. Checks if gravity should be turned off
			if (IsClimbing() || pushAxis.y > 0)
				body.useGravity = false;

			//6. This will keep velocity if consistent when moving while falling
			if (body.useGravity)
				body.velocity = new Vector3(body.velocity.x, yVel, body.velocity.z);

			SyncBodyHead();

			//*moveDirection = Vector3.zero;
			ignoreIterpolationFrame = false;
			lastUpdateTime = Time.realtimeSinceStartup;
		}

		private void SyncBodyHead()
		{
			var delta = 50f * Time.fixedDeltaTime;
			var scale = transform.lossyScale.x > transform.lossyScale.z
				? transform.lossyScale.x
				: transform.lossyScale.z;
			var found = false;
			if ((headCamera.transform.position - transform.position).magnitude > 0.15f)
				for (var i = 0; i < 5; i++)
				{
					var direction = headCamera.transform.position - transform.position;
					direction.y = 0;
					Debug.DrawLine(transform.position, transform.position + direction.normalized * 0.03f, Color.yellow);

					if (!Physics.CapsuleCast(scale * transform.position + Vector3.up * scale * bodyCapsule.radius,
						    transform.position - scale * Vector3.up * bodyCapsule.radius +
						    scale * Vector3.up * bodyCapsule.height, scale * bodyCapsule.radius,
						    direction, 0.03f * delta, handPlayerMask))
					{
						offset = direction * 0.03f * delta;
						transform.position += offset;
						targetTrackedPos -= offset;
						found = true;
					}
					else
					{
						for (var y = -75; y <= 75; y += 15)
						{
							var newDirection = Quaternion.Euler(0, y, 0) * direction;
							Debug.DrawLine(transform.position, transform.position + newDirection.normalized * 0.1f,
								Color.yellow);

							if (!Physics.CapsuleCast(
								    scale * transform.position + Vector3.up * scale * bodyCapsule.radius,
								    transform.position - scale * Vector3.up * bodyCapsule.radius +
								    scale * Vector3.up * bodyCapsule.height, scale * bodyCapsule.radius,
								    newDirection, 0.03f * delta, handPlayerMask))
							{
								offset = newDirection * 0.03f * delta;
								transform.position += offset;
								targetTrackedPos -= offset;
								found = true;
								break;
							}
						}
					}

					if (!found)
						break;
				}
		}

		protected virtual bool CanInputMove()
		{
			return allowClimbingMovement || !IsClimbing();
		}

		protected virtual void InterpolateMovement()
		{
			var deltaTime = Time.realtimeSinceStartup - lastUpdateTime;
			var startRightHandPos = handRight.transform.position;
			var startLeftHandPos = handLeft.transform.position;

			//Smooth moves body based on velocity
			body.position = Vector3.MoveTowards(body.position, body.position + body.velocity,
				body.velocity.magnitude * deltaTime);
			body.velocity = Vector3.MoveTowards(body.velocity, Vector3.zero, body.velocity.magnitude * deltaTime);
			transform.position = body.position;

			if (!ignoreIterpolationFrame)
			{
				//Moves the tracked objects based on the physics bodys delta movement
				targetTrackedPos += transform.position - lastUpdatePosition;
				var flatPos = new Vector3(targetTrackedPos.x, trackingContainer.position.y, targetTrackedPos.z);
				trackingContainer.position = flatPos;

				//This slow moves the head + controllers on the Y-axis so it doesn't jump when stepping up
				trackingContainer.position = Vector3.MoveTowards(trackingContainer.position,
					targetTrackedPos + Vector3.up * heightOffset,
					(Mathf.Abs(trackingContainer.position.y - targetTrackedPos.y) + 0.1f) * Time.deltaTime *
					heightSmoothSpeed);

				//This code will move the tracking objects to match the body collider position when moving
				var targetPos = transform.position - headCamera.transform.position;
				targetPos.y = 0;
				targetPosOffset =
					Vector3.MoveTowards(targetPosOffset, targetPos, body.velocity.magnitude * deltaTime * 2);
				trackingContainer.position += targetPosOffset;

				if (headPhysicsFollower !=
				    null) //Keeps the head down when colliding something above it and manages bouncing back up when not
					if (Vector3.Distance(headCamera.transform.position, headPhysicsFollower.transform.position) >
					    headPhysicsFollower.headCollider.radius / 1.5f)
					{
						var idealPos = headPhysicsFollower.transform.position +
						               (headCamera.transform.position - headPhysicsFollower.transform.position)
						               .normalized * headPhysicsFollower.headCollider.radius / 1.5f;
						var offsetPos = headCamera.transform.position - idealPos;
						trackingContainer.position -= offsetPos;
					}

				//This helps prevent the hands from clipping
				var deltaHandPos = handRight.transform.position - startRightHandPos;
				if (pushRight.Count > 0)
					handRight.transform.position -= deltaHandPos;
				else if (handRight.body.SweepTest(deltaHandPos, out var hitRight, deltaHandPos.magnitude))
					if (handRight.holdingObj == null || (hitRight.rigidbody != handRight.holdingObj.body &&
					                                     !handRight.holdingObj.jointedBodies.Contains(
						                                     hitRight.rigidbody)))
						if (handLeft.holdingObj == null || (hitRight.rigidbody != handLeft.holdingObj.body &&
						                                    !handLeft.holdingObj.jointedBodies.Contains(
							                                    hitRight.rigidbody)))
							handRight.transform.position -= deltaHandPos;
				deltaHandPos = handLeft.transform.position - startLeftHandPos;
				if (pushLeft.Count > 0)
					handLeft.transform.position -= deltaHandPos;
				else if (handLeft.body.SweepTest(deltaHandPos, out var hitLeft, deltaHandPos.magnitude))
					if (handRight.holdingObj == null || (hitLeft.rigidbody != handRight.holdingObj.body &&
					                                     !handRight.holdingObj.jointedBodies
						                                     .Contains(hitLeft.rigidbody)))
						if (handLeft.holdingObj == null || (hitLeft.rigidbody != handLeft.holdingObj.body &&
						                                    !handLeft.holdingObj.jointedBodies.Contains(
							                                    hitLeft.rigidbody)))
							handLeft.transform.position -= deltaHandPos;
			}

			lastUpdatePosition = transform.position;
			lastUpdateTime = Time.realtimeSinceStartup;
		}


		protected virtual void UpdateTurn(float deltaTime)
		{
			//Snap turning
			if (snapTurning)
			{
				if (Mathf.Abs(turningAxis) > turnDeadzone && axisReset)
				{
					var angle = turningAxis > turnDeadzone ? snapTurnAngle : -snapTurnAngle;

					var targetPos = transform.position - headCamera.transform.position;
					targetPos.y = 0;

					trackingContainer.position += targetPos;
					if (headPhysicsFollower != null)
					{
						headPhysicsFollower.transform.position += targetPos;
						headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
					}

					lastUpdatePosition = transform.position;

					trackingContainer.RotateAround(transform.position, Vector3.up, angle);

					targetPosOffset = Vector3.zero;
					targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y,
						trackingContainer.position.z);


					axisReset = false;
				}
			}
			else if (Mathf.Abs(turningAxis) > turnDeadzone)
			{
				var targetPos = transform.position - headCamera.transform.position;
				targetPos.y = 0;

				trackingContainer.position += targetPos;
				if (headPhysicsFollower != null)
				{
					headPhysicsFollower.transform.position += targetPos;
					headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
				}

				lastUpdatePosition = transform.position;

				trackingContainer.RotateAround(headCamera.transform.position, Vector3.up,
					smoothTurnSpeed * (turningAxis - turnDeadzone) * deltaTime);

				targetPosOffset = Vector3.zero;
				targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y,
					trackingContainer.position.z);


				axisReset = false;
			}

			if (Mathf.Abs(turningAxis) < turnResetzone)
				axisReset = true;
		}

		protected virtual void Ground()
		{
			isGrounded = false;
			newClosestHit = new RaycastHit();
			if (useGrounding && !IsClimbing() && !(pushAxis.y > 0))
			{
				highestPoint = -1;
				CheckGroundRadius(2, 0);
				CheckGroundRadius(groundRayCount, 1);
				CheckGroundRadius(groundRayCount / 2, 0.75f);
				CheckGroundRadius(groundRayCount / 3, 0.50f);
				CheckGroundRadius(groundRayCount / 4, 0.25f);

				if (isGrounded)
				{
					body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
					body.position += Vector3.up * (highestPoint - groundedOffset / 2f);
					transform.position = body.position;
				}

				body.useGravity = !isGrounded;

				void CheckGroundRadius(int groundRayCount, float multi)
				{
					RaycastHit stepHit;
					float stepAngle;
					float dist;
					var radius = bodyCapsule.radius;
					var scale = transform.lossyScale.x > transform.lossyScale.z
						? transform.lossyScale.x
						: transform.lossyScale.z;
					Vector3 stepPos;

					for (var i = 0; i < groundRayCount; i++)
					{
						stepPos = transform.position;
						stepPos.x += Mathf.Cos(i * Mathf.PI / (groundRayCount / 2)) * (scale * radius + 0.04f) * multi;
						stepPos.z += Mathf.Sin(i * Mathf.PI / (groundRayCount / 2)) * (scale * radius + 0.04f) * multi;
						stepPos.y += maxStepHeight;
						Debug.DrawRay(stepPos, -Vector3.up * (maxStepHeight + groundedOffset), Color.red,
							Time.fixedDeltaTime);

						if (Physics.Raycast(stepPos, -Vector3.up, out stepHit, maxStepHeight + groundedOffset,
							    groundLayerMask, QueryTriggerInteraction.Ignore))
						{
							stepAngle = Vector3.Angle(stepHit.normal, Vector3.up);
							dist = Vector3.Distance(stepHit.point,
								stepPos - Vector3.up * (maxStepHeight + groundedOffset));
							if (stepAngle < maxStepAngle && dist > highestPoint)
							{
								isGrounded = true;
								highestPoint = dist;
								newClosestHit = stepHit;
							}
						}
					}
				}
			}
		}

		public bool IsGrounded()
		{
			return isGrounded;
		}

		public void ToggleFlying()
		{
			useGrounding = !useGrounding;
			body.useGravity = useGrounding;
		}

		protected virtual void UpdatePlayerHeight()
		{
			if (crouching != lastCrouching)
			{
				if (lastCrouching)
					heightOffset += lastCrouchingHeight;
				if (!lastCrouching)
					heightOffset -= crouchHeight;

				lastCrouching = crouching;
				lastCrouchingHeight = crouchHeight;
			}

			if (autoAdjustColliderHeight)
			{
				playerHeight = Mathf.Clamp(headCamera.transform.position.y - transform.position.y, minMaxHeight.x,
					minMaxHeight.y);
				bodyCapsule.height = playerHeight;
				var centerHeight = playerHeight / 2f > bodyCapsule.radius ? playerHeight / 2f : bodyCapsule.radius;
				bodyCapsule.center = new Vector3(0, centerHeight, 0);
			}
		}


		protected void UpdatePlatform(bool isFixedUpdate)
		{
			if ((!ignoreIterpolationFrame || isFixedUpdate) && isGrounded && newClosestHit.transform != null)
			{
				if (newClosestHit.transform != closestHit.transform)
				{
					closestHit = newClosestHit;
					lastPlatformPosition = closestHit.transform.position;
					lastPlatformRotation = closestHit.transform.rotation;
				}
				else if (newClosestHit.transform == closestHit.transform)
				{
					if (closestHit.transform.position != lastPlatformPosition)
					{
						closestHit = newClosestHit;
						transform.position += closestHit.transform.position - lastPlatformPosition;
						body.position = transform.position;

						var deltaRot = (closestHit.transform.rotation * Quaternion.Inverse(lastPlatformRotation))
							.eulerAngles;
						transform.RotateAround(closestHit.transform.position, Vector3.up, deltaRot.y);
						//transform.RotateAround(closestHit.transform.position, Vector3.right, deltaRot.x);
						//transform.RotateAround(closestHit.transform.position, Vector3.forward, deltaRot.z);
						body.position = transform.position;
						body.rotation = transform.rotation;

						trackingContainer.RotateAround(closestHit.transform.position, Vector3.up, deltaRot.y);
						//trackingContainer.RotateAround(closestHit.transform.position, Vector3.right, deltaRot.x);
						//trackingContainer.RotateAround(closestHit.transform.position, Vector3.forward, deltaRot.z);

						lastPlatformPosition = closestHit.transform.position;
						lastPlatformRotation = closestHit.transform.rotation;
					}
				}
			}
		}


		public void Jump(float jumpPower = 1)
		{
			if (isGrounded)
			{
				DisableGrounding(0.1f);
				body.velocity += Vector3.up * jumpPower;
			}
		}


		public void DisableGrounding(float seconds)
		{
			if (disableGroundingRoutine != null)
				StopCoroutine(disableGroundingRoutine);
			disableGroundingRoutine = StartCoroutine(DisableGroundingSecondsRoutine(seconds));
		}

		private IEnumerator DisableGroundingSecondsRoutine(float seconds)
		{
			useGrounding = false;
			yield return new WaitForSeconds(seconds);
			useGrounding = true;
		}

		/// <summary>Legacy function, use body.addfoce instead</summary>
		public void AddVelocity(Vector3 force, ForceMode mode = ForceMode.Acceleration)
		{
			body.AddForce(force, mode);
		}

		protected virtual void StartPush(Hand hand, GameObject other)
		{
			if (!allowBodyPushing || IsClimbing())
				return;

			if (other.CanGetComponent(out Pushable push) && push.enabled)
			{
				if (hand.left)
				{
					if (!pushLeft.ContainsKey(push))
					{
						pushLeft.Add(push, hand);
						pushLeftCount.Add(push, 1);
					}
					else
					{
						pushLeftCount[push]++;
					}
				}

				if (!hand.left && !pushRight.ContainsKey(push))
				{
					if (!pushRight.ContainsKey(push))
					{
						pushRight.Add(push, hand);
						pushRightCount.Add(push, 1);
					}
					else
					{
						pushRightCount[push]++;
					}
				}
			}
		}

		protected virtual void StopPush(Hand hand, GameObject other)
		{
			if (!allowBodyPushing)
				return;

			if (other.CanGetComponent(out Pushable push))
			{
				if (hand.left && pushLeft.ContainsKey(push))
				{
					var count = --pushLeftCount[push];
					if (count == 0)
					{
						pushLeft.Remove(push);
						pushLeftCount.Remove(push);
					}
				}

				if (!hand.left && pushRight.ContainsKey(push))
				{
					var count = --pushRightCount[push];
					if (count == 0)
					{
						pushRight.Remove(push);
						pushRightCount.Remove(push);
					}
				}
			}
		}

		protected virtual void StartGrabPush(Hand hand, Grabbable grab)
		{
			if (!allowBodyPushing)
				return;

			if (grab.CanGetComponent(out Pushable push) && push.enabled)
			{
				if (hand.left && !pushLeft.ContainsKey(push))
				{
					pushLeft.Add(push, hand);
					pushLeftCount.Add(push, 1);
				}

				if (!hand.left && !pushRight.ContainsKey(push))
				{
					pushRight.Add(push, hand);
					pushRightCount.Add(push, 1);
				}
			}
		}

		protected virtual void EndGrabPush(Hand hand, Grabbable grab)
		{
			if (grab != null && grab.CanGetComponent(out Pushable push))
			{
				if (hand.left && pushLeft.ContainsKey(push))
				{
					pushLeft.Remove(push);
					pushLeftCount.Remove(push);
				}
				else if (!hand.left && pushRight.ContainsKey(push))
				{
					pushRight.Remove(push);
					pushRightCount.Remove(push);
				}
			}
		}

		protected virtual void ApplyPushingForce()
		{
			pushAxis = Vector3.zero;
			if (allowBodyPushing)
			{
				var rightHandCast = Physics.RaycastAll(handRight.transform.position, Vector3.down, 0.1f,
					~handRight.handLayers);
				var leftHandCast =
					Physics.RaycastAll(handLeft.transform.position, Vector3.down, 0.1f, ~handLeft.handLayers);
				var hitObjects = new List<GameObject>();
				foreach (var hit in rightHandCast) hitObjects.Add(hit.transform.gameObject);
				foreach (var hit in leftHandCast) hitObjects.Add(hit.transform.gameObject);

				foreach (var push in pushRight)
					if (push.Key.enabled && !push.Value.IsGrabbing())
					{
						var offset = Vector3.zero;
						var distance = Vector3.Distance(push.Value.body.position, push.Value.moveTo.position);
						if (distance > 0)
							offset = Vector3.Scale(push.Value.body.position - push.Value.moveTo.position,
								push.Key.strengthScale);

						offset = Vector3.Scale(offset, pushingStrength);
						if (!hitObjects.Contains(push.Key.transform.gameObject))
							offset.y = 0;
						pushAxis += offset / 2f;
					}

				foreach (var push in pushLeft)
					if (push.Key.enabled && !push.Value.IsGrabbing())
					{
						var offset = Vector3.zero;
						var distance = Vector3.Distance(push.Value.body.position, push.Value.moveTo.position);
						if (distance > 0)
							offset = Vector3.Scale(push.Value.body.position - push.Value.moveTo.position,
								push.Key.strengthScale);

						offset = Vector3.Scale(offset, pushingStrength);
						if (!hitObjects.Contains(push.Key.transform.gameObject))
							offset.y = 0;
						pushAxis += offset / 2f;
					}
			}
		}

		public bool IsPushing()
		{
			foreach (var push in pushRight)
				if (push.Key.enabled)
					return true;
			foreach (var push in pushLeft)
				if (push.Key.enabled)
					return true;

			return false;
		}


		protected virtual void StartClimb(Hand hand, Grabbable grab)
		{
			if (!allowClimbing)
				return;

			if (!climbing.ContainsKey(hand) && grab != null && grab.CanGetComponent(out Climbable climbbable) &&
			    climbbable.enabled)
			{
				if (climbing.Count == 0)
				{
					pushRight.Clear();
					pushRightCount.Clear();
					pushLeft.Clear();
					pushLeftCount.Clear();
				}

				climbing.Add(hand, climbbable);
			}
		}

		protected virtual void EndClimb(Hand hand, Grabbable grab)
		{
			if (!allowClimbing)
				return;

			if (climbing.ContainsKey(hand))
				climbing.Remove(hand);
		}

		protected virtual void ApplyClimbingForce()
		{
			climbAxis = Vector3.zero;
			if (allowClimbing && climbing.Count > 0)
				foreach (var hand in climbing)
					if (hand.Value.enabled)
					{
						var offset = Vector3.Scale(hand.Key.body.position - hand.Key.moveTo.position, hand.Value.axis);
						offset = Vector3.Scale(offset, climbingStrength);
						climbAxis += offset / climbing.Count;
					}
		}

		public bool IsClimbing()
		{
			foreach (var climb in climbing)
				if (climb.Value.enabled)
					return true;
			return false;
		}


		public virtual void SetPosition(Vector3 position)
		{
			SetPosition(position, headCamera.transform.rotation);
		}

		public virtual void SetPosition(Vector3 position, Quaternion rotation)
		{
			var deltaPos = position - transform.position;
			transform.position += deltaPos;
			//This code will move the tracking objects to match the body collider position when moving
			var targetPos = transform.position - headCamera.transform.position;
			targetPos.y = deltaPos.y;
			trackingContainer.position += targetPos;
			lastUpdatePosition = transform.position;
			targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y + deltaPos.y,
				trackingContainer.position.z);
			targetPosOffset = Vector3.zero;
			body.position = transform.position;
			if (headPhysicsFollower != null)
			{
				headPhysicsFollower.transform.position += targetPos;
				headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
			}

			var deltaRot = rotation * Quaternion.Inverse(headCamera.transform.rotation);
			trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, deltaRot.eulerAngles.y);
			//trackingContainer.RotateAround(headCamera.transform.position, Vector3.right, deltaRot.eulerAngles.x);
			//trackingContainer.RotateAround(headCamera.transform.position, Vector3.forward, deltaRot.eulerAngles.z);
		}

		public virtual void SetRotation(Quaternion rotation)
		{
			var targetPos = transform.position - headCamera.transform.position;
			targetPos.y = 0;

			trackingContainer.position += targetPos;
			if (headPhysicsFollower != null)
			{
				headPhysicsFollower.transform.position += targetPos;
				headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
			}

			lastUpdatePosition = transform.position;

			var deltaRot = rotation * Quaternion.Inverse(headCamera.transform.rotation);
			trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, deltaRot.eulerAngles.y);
			//trackingContainer.RotateAround(headCamera.transform.position, Vector3.right, deltaRot.eulerAngles.x);
			//trackingContainer.RotateAround(headCamera.transform.position, Vector3.forward, deltaRot.eulerAngles.z);

			targetPosOffset = Vector3.zero;
			targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y,
				trackingContainer.position.z);
		}

		public virtual void AddRotation(Quaternion addRotation)
		{
			var targetPos = transform.position - headCamera.transform.position;
			targetPos.y = 0;

			trackingContainer.position += targetPos;
			if (headPhysicsFollower != null)
			{
				headPhysicsFollower.transform.position += targetPos;
				headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
			}

			lastUpdatePosition = transform.position;

			trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, addRotation.eulerAngles.y);
			//trackingContainer.RotateAround(headCamera.transform.position, Vector3.right, addRotation.eulerAngles.x);
			//trackingContainer.RotateAround(headCamera.transform.position, Vector3.forward, addRotation.eulerAngles.z);

			targetPosOffset = Vector3.zero;
			targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y,
				trackingContainer.position.z);
		}

		public virtual void Recenter()
		{
			var targetPos = transform.position - headCamera.transform.position;
			targetPos.y = 0;

			trackingContainer.position += targetPos;
			if (headPhysicsFollower != null)
			{
				headPhysicsFollower.transform.position += targetPos;
				headPhysicsFollower.body.position = headPhysicsFollower.transform.position;
			}

			lastUpdatePosition = transform.position;

			targetPosOffset = Vector3.zero;
			targetTrackedPos = new Vector3(trackingContainer.position.x, targetTrackedPos.y,
				trackingContainer.position.z);
		}


		private Vector3 AlterDirection(Vector3 moveAxis)
		{
			if (useGrounding)
				return Quaternion.AngleAxis(forwardFollow.eulerAngles.y, Vector3.up) *
				       new Vector3(moveAxis.x, moveAxis.y, moveAxis.z);
			return forwardFollow.rotation * new Vector3(moveAxis.x, moveAxis.y, moveAxis.z);
		}
	}
}