using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silkroad_Fusion
{
    class _Skills
    {
        public struct skill
        {
            public uint uPk2Id;

            public skill(uint _uPk2Id)
            {
                uPk2Id = _uPk2Id;
            }
        }

        public List<skill> _skill = new List<skill>();

        public void Add(uint uPk2Id)
        {
            _skill.Add(new skill(uPk2Id));
        }
    }

    class _Masteries
    {
        public Dictionary<uint, byte> masteries = new Dictionary<uint, byte>();

        public void Add(uint pk2id,
                        byte level
                        )
        {
            masteries.Add(pk2id, level);
        }

        public void Change(uint pk2id, byte level)
        {
            masteries[pk2id] = level;
        }

        public void Clear()
        {
            masteries.Clear();
        }
    }
}
