
Shader "Custom/StencilMask"
{
    Properties
    {
        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0 // �� ��������� � ���������� ��������� �������� Stencil ID
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "Queue"="Geometry-1" "RenderPipeline" = "UniversalPipeline"} // "Queue"="Geometry-1" - ������� �� ������, ���� �������� � ����� ������������ ������ ����

        Pass 
        {
            Blend Zero One
            ZWrite Off // �������� Z �����, ��� �� �� ������� ������ ������ �� ����������

            Stencil // ��������
            {
                Ref [_StencilID]
                Comp Always
                Pass Replace // �������� �� ������, ��� ���������� �����, ��������� �������� ������ (0) �� �������� _StencilID
            }
        }
    }
}
