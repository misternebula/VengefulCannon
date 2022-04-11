using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VengefulCannon
{
    public class Class1 : ModBehaviour
    {
        private bool _hasHitShip;
        private OWRigidbody ship;
        private OrbitalProbeLaunchController launchController;
        private OWRigidbody probe;

        public void Awake()
		{
            LoadManager.OnCompleteSceneLoad += OnSceneLoad;
		}

        void OnSceneLoad(OWScene oldScene, OWScene newScene)
		{
            _hasHitShip = false;
		}

        public void FixedUpdate()
		{
            if (LoadManager.GetCurrentScene() != OWScene.SolarSystem)
			{
                return;
			}

            if (_hasHitShip)
			{
                return;
			}

            if (launchController == null)
			{
                launchController = Resources.FindObjectsOfTypeAll<OrbitalProbeLaunchController>().First();
            }

            if (!launchController._hasLaunchedProbe)
			{
                return;
			}

            if (ship == null)
			{
                ship = Locator.GetShipBody();
			}

            if (probe == null)
			{
                probe = launchController._probeBody;
                var align = probe.gameObject.AddComponent<AlignWithTargetBody>();
                align.SetTargetBody(Locator.GetShipBody());
                align.SetUsePhysicsToRotate(true);
                align.SetLocalAlignmentAxis(new Vector3(-1, 0, 0));
			}

            var approachSpeed = probe.GetVelocity().magnitude - ship.GetVelocity().magnitude;

            var newSpeed = Mathf.Max(500, ship.GetVelocity().magnitude + 500f);

            probe.SetVelocity(probe.transform.TransformDirection(new Vector3(-1, 0, 0) * newSpeed));

            if (Vector3.Distance(ship.GetPosition(), probe.GetPosition()) <= 50f)
			{
                ModHelper.Console.WriteLine($"HIT SHIP!");
                _hasHitShip = true;
                probe.GetComponent<AlignWithTargetBody>().enabled = false;
			}
		}
    }
}
