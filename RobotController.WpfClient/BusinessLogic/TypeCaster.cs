using System.ComponentModel;

namespace RobotController.WpfGui.BusinessLogic
{
    public class TypeCaster
    { 
        public static object Cast(string input, ESendingType type)
        {
            object data;

            switch (type)
            {
                case ESendingType.Float:
                    data = float.Parse(input);
                    break;

                case ESendingType.Double:
                    data = double.Parse(input);
                    break;

                case ESendingType.Uint8:
                    data = byte.Parse(input);
                    break;

                case ESendingType.Uint16:
                    data = ushort.Parse(input);
                    break;

                case ESendingType.Uint32:
                    data = uint.Parse(input);
                    break;

                case ESendingType.Int8:
                    data = sbyte.Parse(input);
                    break;

                case ESendingType.Int16:
                    data = short.Parse(input);
                    break;

                case ESendingType.Int32:
                    data = int.Parse(input);
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }

            return data;
        }
    }
}
