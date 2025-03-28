using Camera2.Interfaces;
using Camera2.Utils;
using UnityEngine;
using Newtonsoft.Json;

namespace Camera2.Configuration {
	class Settings_FollowAvatar : CameraSubSettings {
		public bool followAvatar = false;
		public float smoothing = 5f;


		[JsonConverter(typeof(Vector3Converter))]
		public Vector3 offset = Vector3.zero;
	}
}

namespace Camera2.Middlewares {
	class FollowAvatar : CamMiddleware, IMHandler {

		Transformer transformer;

		new public bool Pre() {
			if(!settings.FollowAvatar.followAvatar || settings.type == Configuration.CameraType.FirstPerson)
				return true;

			transformer ??= cam.transformchain.AddOrGet("FollowAvatar", TransformerOrders.FollowAvatar, true);

			Transform theTransform = cam.UCamera.transform;
			Transform target = Camera.main.transform;

			var relativePosition = target.position + settings.FollowAvatar.offset - theTransform.position;
			var rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
			rotation = Quaternion.Slerp(transformer.rotation, rotation, cam.timeSinceLastRender * settings.FollowAvatar.smoothing);

			transformer.applyAsAbsoluteRotation = true;

			transformer.rotation = rotation;

			return true;
		}
	}
}