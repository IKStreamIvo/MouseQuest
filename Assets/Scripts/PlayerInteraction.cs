using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    public float grappleRange = 50f;
    public LayerMask grappleableObjectsLayer;
    public float grapplingDistanceThreshold = .2f;
    public float grapplingMoveSpeed = 10f;
    private new Camera camera;
    private bool isGrappling;
    private bool hasTarget;
    private Vector3 grappleTargetPosition;
    private new CapsuleCollider collider;
    private new Rigidbody rigidbody;
    public float jumpWhileGrapplingUpForce = 10f;
    private Vector3 grapplingSpeedSpeed;
    private Collider targetGrappleObject;
    private float grapplingTimer;
    public float grapplingTimerHittingAnyDelay = 2f;
    public float shootGrappleDelay = 2f;
    private float grapplingFireTimer;

    private void Start() {
        camera = Camera.main;
        collider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        grapplingFireTimer = shootGrappleDelay;
    }

    void Update() {
        if (!isGrappling && grapplingFireTimer >= shootGrappleDelay) {
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, grappleRange, grappleableObjectsLayer)) { //try find a target
                hasTarget = true;

                if (Input.GetMouseButtonDown(0)) { //if we have a target and click, shoot a hook
                    grappleTargetPosition = hit.point;
                    targetGrappleObject = hit.collider;
                    isGrappling = true;
                    rigidbody.useGravity = false;
                    Vector3 direction = (grappleTargetPosition - transform.position).normalized;
                    rigidbody.velocity = direction * grapplingMoveSpeed;
                    grapplingTimer = 0f;
                }
            } else {
                hasTarget = false;
            }
        } else {
            grapplingFireTimer += Time.deltaTime;
        }

        if (isGrappling && targetGrappleObject != null) {
            grapplingTimer += Time.deltaTime;
            //check if we're there
            bool hitTarget = false;
            Collider[] hits = Physics.OverlapSphere(transform.position, collider.radius + grapplingDistanceThreshold, grappleableObjectsLayer);
            //if we're still within our delay, check if we're at our target
            if (grapplingTimer < grapplingTimerHittingAnyDelay) {
                for (int i = 0; i < hits.Length; i++) {
                    if (hits[i] == targetGrappleObject) {
                        hitTarget = true;
                    }
                }
            } else { //otherwise check if we just hit anything
                hitTarget = hits.Length > 0;
            }

            if (hitTarget) {
                isGrappling = false;
                rigidbody.useGravity = true;
                grapplingFireTimer = 0f;
                rigidbody.AddForce(Vector3.up * jumpWhileGrapplingUpForce/2f);
            } else { //move towards point
                Vector3 direction = (grappleTargetPosition - transform.position).normalized;
                rigidbody.velocity = direction * grapplingMoveSpeed;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                isGrappling = false;
                rigidbody.useGravity = true;
                rigidbody.AddForce(Vector3.up * jumpWhileGrapplingUpForce);
            }
        }
    }
}
