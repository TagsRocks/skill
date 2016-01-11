using UnityEngine;

namespace Skill.Framework
{
    public struct SafeInt
    {
        private bool _IsSafed;
        private int _MixKey;
        private int _Value;

        public SafeInt(int value = 0)
        {
            _IsSafed = true;
            _MixKey = Random.Range(9876, 987654321);
            this._Value = value ^ _MixKey;
        }

        public int Value
        {
            get
            {
                if (!_IsSafed)
                {
                    _IsSafed = true;
                    _MixKey = Random.Range(14673, 9943658);
                    _Value = 0 ^ _MixKey;
                }
                return _Value ^ _MixKey;
            }
        }

        public int Key { get { return _MixKey; } }
        public int MixedValue { get { return _Value; } }
        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj) { return Value.Equals(obj); }

        public override string ToString() { return Value.ToString(); }
        public static SafeInt operator +(SafeInt i1, SafeInt i2) { return new SafeInt(i1.Value + i2.Value); }
        public static SafeInt operator -(SafeInt i1, SafeInt i2) { return new SafeInt(i1.Value - i2.Value); }
        public static SafeInt operator *(SafeInt i1, SafeInt i2) { return new SafeInt(i1.Value * i2.Value); }
        public static SafeInt operator /(SafeInt i1, SafeInt i2) { return new SafeInt(i1.Value / i2.Value); }

        public static bool operator ==(SafeInt x, SafeInt y) { return x.Value == y.Value; }
        public static bool operator !=(SafeInt x, SafeInt y) { return x.Value != y.Value; }


        public static bool operator <(SafeInt i1, SafeInt i2) { return i1.Value < i2.Value; }
        public static bool operator >(SafeInt i1, SafeInt i2) { return i1.Value > i2.Value; }

        public static bool operator <=(SafeInt i1, SafeInt i2) { return i1.Value <= i2.Value; }
        public static bool operator >=(SafeInt i1, SafeInt i2) { return i1.Value >= i2.Value; }

        public static SafeInt operator ++(SafeInt i1) { return new SafeInt(i1.Value + 1); }
        public static SafeInt operator --(SafeInt i1) { return new SafeInt(i1.Value - 1); }
        public static SafeInt operator -(SafeInt i1) { return new SafeInt(i1.Value * -1); }

        public static implicit operator SafeInt(int i) { return new SafeInt(i); }

        public static implicit operator int(SafeInt si) { return si.Value; }
    }
    public struct SafeFloat
    {
        private bool _IsSafed;
        private float _Offset;
        private float _Value;

        public SafeFloat(float value = 0)
        {
            _IsSafed = true;
            _Offset = Random.Range(-1000, 1000);
            this._Value = value + _Offset;
        }

        public float Value
        {
            get
            {
                if (!_IsSafed)
                {
                    _IsSafed = true;
                    _Offset = Random.Range(-1000, 1000);
                    this._Value = 0 + _Offset;
                }
                return _Value - _Offset;
            }
        }

        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj) { return Value.Equals(obj); }
        public override string ToString() { return Value.ToString(); }
        public static SafeFloat operator +(SafeFloat f1, SafeFloat f2) { return new SafeFloat(f1.Value + f2.Value); }
        public static SafeFloat operator -(SafeFloat f1, SafeFloat f2) { return new SafeFloat(f1.Value - f2.Value); }
        public static SafeFloat operator *(SafeFloat f1, SafeFloat f2) { return new SafeFloat(f1.Value * f2.Value); }
        public static SafeFloat operator /(SafeFloat f1, SafeFloat f2) { return new SafeFloat(f1.Value / f2.Value); }

        public static bool operator ==(SafeFloat x, SafeFloat y) { return x.Value == y.Value; }
        public static bool operator !=(SafeFloat x, SafeFloat y) { return x.Value != y.Value; }

        public static bool operator <(SafeFloat i1, SafeFloat i2) { return i1.Value < i2.Value; }
        public static bool operator >(SafeFloat i1, SafeFloat i2) { return i1.Value > i2.Value; }

        public static bool operator <=(SafeFloat i1, SafeFloat i2) { return i1.Value <= i2.Value; }
        public static bool operator >=(SafeFloat i1, SafeFloat i2) { return i1.Value >= i2.Value; }


        public static SafeFloat operator ++(SafeFloat f1) { return new SafeFloat(f1.Value + 1); }
        public static SafeFloat operator --(SafeFloat f1) { return new SafeFloat(f1.Value - 1); }
        public static SafeFloat operator -(SafeFloat f1) { return new SafeFloat(f1.Value * -1); }


        public static implicit operator SafeFloat(float f) { return new SafeFloat(f); }

        public static implicit operator float(SafeFloat sf) { return sf.Value; }

    }

    public struct SafeBool
    {
        private bool _IsSafed;
        private int _Bit;
        private int _Value;

        public SafeBool(bool value = false)
        {
            _IsSafed = true;
            _Bit = Random.Range(1, 31);
            this._Value = (value) ? 1 << _Bit : 0;
        }

        public bool Value
        {
            get
            {
                if (!_IsSafed)
                {
                    _IsSafed = true;
                    _Bit = Random.Range(1, 31);
                    this._Value = 0;
                }
                return (_Value & (1 << _Bit)) == (1 << _Bit);
            }
        }

        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj) { return Value.Equals(obj); }

        public override string ToString() { return Value.ToString(); }

        public static bool operator ==(SafeBool x, SafeBool y) { return x.Value == y.Value; }
        public static bool operator !=(SafeBool x, SafeBool y) { return x.Value != y.Value; }

        public static implicit operator SafeBool(bool b) { return new SafeBool(b); }

        public static implicit operator bool(SafeBool sb) { return sb.Value; }



    }
}