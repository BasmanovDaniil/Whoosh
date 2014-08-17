using System.Linq;
using UnityEngine;

namespace ProceduralToolkit
{
    public enum SignalType
    {
        Sine,
        Square,
        Triangle,
        Sawtooth
    }

    public enum Note : uint
    {
        C0 = 16352,
        D0 = 18354,
        E0 = 20602,
        F0 = 21827,
        G0 = 24500,
        A0 = 27500,
        H0 = 30868,

        C1 = 32703,
        D1 = 36708,
        E1 = 41203,
        F1 = 43654,
        G1 = 48999,
        A1 = 55000,
        H1 = 61735,

        C2 = 65406,
        D2 = 73416,
        E2 = 82406,
        F2 = 87307,
        G2 = 97999,
        A2 = 110000,
        H2 = 123470,

        C3 = 130810,
        C3D = 138590,
        D3B = 138590,
        D3 = 146830,
        D3D = 155560,
        E3B = 155560,
        E3 = 164810,
        F3 = 174610,
        F3D = 185000,
        G3B = 185000,
        G3 = 196000,
        G3D = 207650,
        A3B = 207650,
        A3 = 220000,
        A3D = 233080,
        H3B = 233080,
        H3 = 246940,

        C4 = 261630,
        D4 = 293670,
        E4 = 329630,
        F4 = 349230,
        G4 = 392000,
        A4 = 440000,
        H4 = 493880,

        C5 = 523250,
        D5 = 587330,
        E5 = 659260,
        F5 = 698460,
        G5 = 783990,
        A5 = 880000,
        H5 = 987770,

        C6 = 1046500,
        D6 = 1174700,
        E6 = 1318500,
        F6 = 1396900,
        G6 = 1568000,
        A6 = 1760000,
        H6 = 1975500,

        C7 = 2093000,
        D7 = 2349300,
        E7 = 2637000,
        F7 = 2793800,
        G7 = 3136000,
        A7 = 3520000,
        H7 = 3951100,

        C8 = 4186000,
        D8 = 4698600,
        E8 = 5274000,
        F8 = 5587700,
        G8 = 6271900,
        A8 = 7040000,
        H8 = 7902100,
    }

    public static class Audio
    {
        public static AudioClip Create(float[] data, string name, int lengthSamples = 44100, int channels = 1,
            int frequency = 44100,
            bool _3D = false, bool stream = false)
        {
            var clip = AudioClip.Create(name, lengthSamples, channels, frequency, _3D, stream);
            clip.SetData(data, 0);
            return clip;
        }

        public static float[] Volume(float[] data, float volume)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= volume;
            }
            return data;
        }

        public static AudioClip Volume(AudioClip clip, float volume)
        {
            var data = new float[clip.samples];
            clip.GetData(data, 0);

            data = Volume(data, volume);

            var filtered = AudioClip.Create(clip.name, clip.samples, 1, clip.frequency, false, false);
            filtered.SetData(data, 0);
            return filtered;
        }

        public static float[] Mix(float[] a, float[] b)
        {
            if (b.Length > a.Length)
            {
                var tmp = a;
                a = b;
                b = tmp;
            }

            for (int i = 0; i < b.Length; i++)
            {
                a[i] = (a[i] + b[i]);
            }

            return a;
        }

        public static float[] Mix(float[] a, float[] b, int offset)
        {
            var data = new float[offset + b.Length];

            for (int i = 0; i < data.Length; i++)
            {
                var v = i < a.Length ? a[i] : 0;
                var w = ((i - offset > 0) && (i - offset < b.Length)) ? b[i - offset] : 0;
                data[i] = v + w;
            }

            return data;
        }

        public static AudioClip Mix(AudioClip a, AudioClip b)
        {
            var aData = new float[a.samples];
            a.GetData(aData, 0);

            var bData = new float[b.samples];
            b.GetData(bData, 0);

            var mixData = Mix(aData, bData);

            var clip = AudioClip.Create("Mix(" + a.name + ", " + b.name + ")", mixData.Length, 1, a.frequency, false,
                false);
            clip.SetData(mixData, 0);
            return clip;
        }

        public static float[] Append(float[] a, float[] b)
        {
            return a.Concat(b).ToArray();
        }

        public static float[] MedianFilter(float[] data, int window)
        {
            int half = window/2;
            for (int i = 0; i < data.Length; i++)
            {
                float sum = 0;
                for (int j = i - half; j < i + half; j++)
                {
                    if (j < 0)
                    {
                        sum += data[0];
                    }
                    else if (j >= data.Length)
                    {
                        sum += data[data.Length - 1];
                    }
                    else
                    {
                        sum += data[j];
                    }
                }

                data[i] = sum/window;
            }
            return data;
        }

        public static AudioClip MedianFilter(AudioClip clip, int window)
        {
            var data = new float[clip.samples];
            clip.GetData(data, 0);

            data = MedianFilter(data, window);

            var filtered = AudioClip.Create("MedianFilter(" + clip.name + ")", clip.samples, 1, clip.frequency, false,
                false);
            filtered.SetData(data, 0);
            return filtered;
        }

        public static AudioClip DoubleNote(Note a, Note b, SignalType signalType = SignalType.Sawtooth,
            float amplitude = 1, int samples = 44100, int length = 1)
        {
            var data = Mix(NoteData(a, signalType, amplitude, samples, length),
                NoteData(b, signalType, amplitude, samples, length));

            return Create(data, a.ToString() + b.ToString(), samples*length, 1, samples, false, false);
        }

        public static AudioClip Chord(params Note[] notes)
        {
            var data = NoteData(notes[0]);
            for (int i = 1; i < notes.Length; i++)
            {
                data = Mix(data, NoteData(notes[i]));
            }

            return Create(data, notes.ToString(), data.Length);
        }

        public static AudioClip Arpeggio(params Note[] notes)
        {
            var data = NoteData(notes[0], samples:22050);
            for (int i = 1; i < notes.Length; i++)
            {
                data = Mix(data, NoteData(notes[i], SignalType.Sawtooth, 1, 22050), 11025 * i);
            }

            return Create(data, notes.ToString(), data.Length);
        }

        public static AudioClip Note(Note note, SignalType signalType = SignalType.Sawtooth, float amplitude = 1,
            int samples = 44100, int length = 1)
        {
            var data = new float[samples*length];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = SignalValue(signalType, (float) note*length/1000, (float) i/data.Length, amplitude);
            }

            return Create(data, note.ToString(), samples*length, 1, samples, false, false);
        }

        public static AudioClip Note(float frequency, SignalType signalType = SignalType.Sawtooth, float amplitude = 1,
            int samples = 44100, int length = 1)
        {
            var data = new float[samples*length];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = SignalValue(signalType, frequency*length, (float) i/data.Length, amplitude);
            }

            return Create(data, frequency.ToString(), samples*length, 1, samples, false, false);
        }

        public static float[] NoteData(float frequency, SignalType signalType = SignalType.Sawtooth, float amplitude = 1,
            int samples = 44100, int length = 1)
        {
            var data = new float[samples*length];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = SignalValue(signalType, frequency*length, (float) i/data.Length, amplitude);
            }

            return data;
        }

        public static float[] NoteData(Note note, SignalType signalType = SignalType.Sawtooth, float amplitude = 1,
            int samples = 44100, int length = 1)
        {
            var data = new float[samples*length];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = SignalValue(signalType, (float) note*length/1000, (float) i/data.Length, amplitude);
            }

            return data;
        }

        public static float SignalValue(SignalType signalType, float frequency, float time, float amplitude)
        {
            float value = 0f;
            float t = frequency*time;
            switch (signalType)
            {
                case SignalType.Sine: // sin( 2 * pi * t )
                    value = Mathf.Sin(2f*Mathf.PI*t);
                    break;
                case SignalType.Square: // sign( sin( 2 * pi * t ) )
                    value = Mathf.Sign(Mathf.Sin(2f*Mathf.PI*t));
                    break;
                case SignalType.Triangle:
                    // 2 * abs( t - 2 * floor( t / 2 ) - 1 ) - 1
                    value = 1f - 4f*Mathf.Abs(Mathf.Round(t - 0.25f) - (t - 0.25f));
                    break;
                case SignalType.Sawtooth:
                    // 2 * ( t/a - floor( t/a + 1/2 ) )
                    value = 2f*(t - Mathf.Floor(t + 0.5f));
                    break;
            }

            return amplitude*value;
        }

        public static AudioClip WhiteNoise(float amplitude, int samples, int length)
        {
            var data = new float[samples*length];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = Random.value*amplitude;
            }

            return Create(data, "White noise", samples*length, 1, samples, false, false);
        }
    }
}