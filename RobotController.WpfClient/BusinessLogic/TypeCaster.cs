using System.ComponentModel;

namespace RobotController.WpfGui.BusinessLogic
{
    public class TypeCaster
    { 
        private readonly string _input;
        private readonly ESendingType _type;

        public TypeCaster(string input, ESendingType type)
        {
            _input = input;
            _type = type;
        }

        public object Cast()
        {
            object data;

            switch (_type)
            {
                case ESendingType.Float:
                    data = float.Parse(_input);
                    break;

                case ESendingType.Double:
                    data = double.Parse(_input);
                    break;

                case ESendingType.Uint8:
                    data = byte.Parse(_input);
                    break;

                case ESendingType.Uint16:
                    data = ushort.Parse(_input);
                    break;

                case ESendingType.Uint32:
                    data = uint.Parse(_input);
                    break;

                case ESendingType.Int8:
                    data = sbyte.Parse(_input);
                    break;

                case ESendingType.Int16:
                    data = short.Parse(_input);
                    break;

                case ESendingType.Int32:
                    data = int.Parse(_input);
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }

            return data;
        }
    }
}
