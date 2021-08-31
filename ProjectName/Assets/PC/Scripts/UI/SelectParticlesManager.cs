using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Assets.PC
{
    [RequireComponent(typeof(ParticleSystem))]
    public class SelectParticlesManager : SingletonBehaviour<SelectParticlesManager>
    {
        private new ParticleSystem particleSystem;
        private MainModule particleModule;

        private void Start() {
            particleSystem = this.GetComponent<ParticleSystem>();
            particleModule = particleSystem.main;
        }

        public void Shoot(Vector3 position, Color color){
            this.transform.position = position;
            particleModule.startColor = color;
            particleSystem.Play();
        }
    }
}
