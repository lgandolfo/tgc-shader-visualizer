using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Shaders;

namespace TGC.Group.Model
{
    class HotReloadEffect
    {
        private string path;
        private byte[] actualHash;
        private float frequency, updateTimer = 0f;
        private Effect effect;

        public HotReloadEffect(Effect effect, string path, float frequency)
        {
            this.path = path;
            this.frequency = frequency;
            this.effect = effect;
            actualHash = GetHash();
        }

        public void Update(float elapsedTime)
        {
            updateTimer += elapsedTime;
            if (updateTimer > frequency)
                HotReload();
        }

        private void HotReload()
        {
            var hash = GetHash();

            if (!Enumerable.SequenceEqual(hash, actualHash))
            {
                ReloadEffect();
                actualHash = hash;
            }
        }

        public Effect Effect => effect;

        private void ReloadEffect()
        {
            effect.Dispose();
            effect = TGCShaders.Instance.LoadEffect(path);
        }

        private byte[] GetHash()
        {
            HashAlgorithm sha1 = HashAlgorithm.Create();
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return sha1.ComputeHash(stream);
        }
    }
}
