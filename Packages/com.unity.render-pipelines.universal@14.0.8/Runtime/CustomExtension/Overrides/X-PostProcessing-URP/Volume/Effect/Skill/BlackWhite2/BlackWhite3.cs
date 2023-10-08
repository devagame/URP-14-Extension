using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    //[VolumeComponentMenu(VolumeDefine.Skill + "黑白闪 (BlackWhite3)")]
    public class BlackWhite3 : VolumeSetting
    {
       public  enum Mask
        {
            R = 0,
            A = 1,
        }
       public  enum Blur_X
       {
           Material = 0,
           Custom_x = 1,
       }
       public  enum Blur_Y
       {
           Material = 0,
           Custom_y = 1,
       }
       
       public  enum BlackWhite
       {
           Material = 0,
           Custom = 1,
       }
       
       public  enum BlackWhiteSwtich
       {
           Material = 0,
           ParticleAlphy = 1,
       }
       
       public  enum Polay
       {
           U = 0,
           V = 1,
       }
       
       public  enum BWMove
       {
           Material = 0,
           Custom1z = 1,
       }
       
       public  enum Shake
       {
           Partercal = 0,
           Material = 1,
       }
       
        [Serializable, DebuggerDisplay(k_DebuggerDisplay)]
        public class MaskEnumParameter : VolumeParameter<Mask>
        {
            public MaskEnumParameter(Mask value, bool overrideState = false)
                : base(value, overrideState) { }
            public sealed override void Interp(Mask from, Mask to, float t)
            {
                m_Value = from;
            }
        }
        
        [Serializable, DebuggerDisplay(k_DebuggerDisplay)]
        public class Blur_X_EnumParameter : VolumeParameter<Blur_X>
        {
            public Blur_X_EnumParameter(Blur_X value, bool overrideState = false)
                : base(value, overrideState) { }
            public sealed override void Interp(Blur_X from, Blur_X to, float t)
            {
                m_Value = from;
            }
        }
        
        [Serializable, DebuggerDisplay(k_DebuggerDisplay)]
        public class Blur_Y_EnumParameter : VolumeParameter<Blur_Y>
        {
            public Blur_Y_EnumParameter(Blur_Y value, bool overrideState = false)
                : base(value, overrideState) { }
            public sealed override void Interp(Blur_Y from, Blur_Y to, float t)
            {
                m_Value = from;
            }
        }
        
        [Serializable, DebuggerDisplay(k_DebuggerDisplay)]
        public class BlackWhiteEnumParameter : VolumeParameter<BlackWhite>
        {
            public BlackWhiteEnumParameter(BlackWhite value, bool overrideState = false)
                : base(value, overrideState) { }
            public sealed override void Interp(BlackWhite from, BlackWhite to, float t)
            {
                m_Value = from;
            }
        }
        
        [Serializable, DebuggerDisplay(k_DebuggerDisplay)]
        public class BlackWhiteSwtichEnumParameter : VolumeParameter<BlackWhiteSwtich>
        {
            public BlackWhiteSwtichEnumParameter(BlackWhiteSwtich value, bool overrideState = false)
                : base(value, overrideState) { }
            public sealed override void Interp(BlackWhiteSwtich from, BlackWhiteSwtich to, float t)
            {
                m_Value = from;
            }
        }
        
        [Serializable, DebuggerDisplay(k_DebuggerDisplay)]
        public class PolayEnumParameter : VolumeParameter<Polay>
        {
            public PolayEnumParameter(Polay value, bool overrideState = false)
                : base(value, overrideState) { }
            public sealed override void Interp(Polay from, Polay to, float t)
            {
                m_Value = from;
            }
        }
        
        [Serializable, DebuggerDisplay(k_DebuggerDisplay)]
        public class BWMoveEnumParameter : VolumeParameter<BWMove>
        {
            public BWMoveEnumParameter(BWMove value, bool overrideState = false)
                : base(value, overrideState) { }
            public sealed override void Interp(BWMove from, BWMove to, float t)
            {
                m_Value = from;
            }
        }
        
        [Serializable, DebuggerDisplay(k_DebuggerDisplay)]
        public class ShakeEnumParameter : VolumeParameter<Shake>
        {
            public ShakeEnumParameter(Shake value, bool overrideState = false)
                : base(value, overrideState) { }
            public sealed override void Interp(Shake from, Shake to, float t)
            {
                m_Value = from;
            }
        }
        
        public override bool IsActive() => Enable.value;

        public BoolParameter Enable = new(false);
       
        //***************************************************************************************//
        [Space(5),Header("色阶调整__________________________________________________")]
        [InspectorName("色阶黑")]public FloatParameter _Float20 = new(0);
        [InspectorName("色阶白")]public FloatParameter _Float19 = new(1);
        [InspectorName("暗角强度")]public FloatParameter _Float28 = new(0);
        [InspectorName("暗角阈值")]public ClampedFloatParameter _Float36 = new(0,0,1);
        [InspectorName("中心位置U")]public ClampedFloatParameter _Float6 = new(0,-0.5f,0.5f);
        [InspectorName("中心位置V")]public ClampedFloatParameter _Float7 = new(0,-0.5f,0.5f);

        [Space(5),Header("Mask遮罩__________________________________________________")]
        [InspectorName("Mask")]public TextureParameter _TextureSample1 = new(null);
        [InspectorName("_TextureSample1_ST")]public Vector4Parameter _TextureSample1_ST = new(new Vector4(1,1,0,0));
        [InspectorName("Mask通道")]public MaskEnumParameter _Float30 = new(0);
        [InspectorName("Mask反转")]public BoolParameter _Float24 = new(false);
        [InspectorName("Mask平方")]public FloatParameter _MaskPower1 = new(1);
        [InspectorName("Msak强度")]public FloatParameter _Float9 = new(1);
        
        [Space(5),Header("色散模糊__________________________________________________")]
        [InspectorName("色散强度")]public FloatParameter _Float11 = new(0);
        [InspectorName("色散控制方式")]public Blur_X_EnumParameter _Float17 = new(0);
        [InspectorName("模糊缩放")]public FloatParameter _Float5 = new(0);
        [InspectorName("模糊控制方式")]public Blur_Y_EnumParameter _Float18 = new(0);
        [InspectorName("色散模糊比重")]public ClampedFloatParameter _Float8 = new(1,0,1);
        [InspectorName("放射纹理强度")]public FloatParameter _Float27 = new(0);
        
        [Space(5),Header("黑白闪__________________________________________________")]
        [InspectorName("黑白闪开关")]public BoolParameter _Float4 = new(false);
        [InspectorName("黑白闪开关控制方式")]public BlackWhiteEnumParameter _Float10 = new(0);
        [InspectorName("黑白闪颜色1")]public ColorParameter _Color0 = new(Color.white);
        [InspectorName("黑白闪颜色2")]public ColorParameter _Color1 = new(Color.black);
        [InspectorName("黑白切换")]public BoolParameter _Float33 = new(false);
        [InspectorName("黑白切换控制方式")]public BlackWhiteSwtichEnumParameter _Float34 = new(0);
        
        [InspectorName("黑白闪纹理")]public TextureParameter _TextureSample0 = new(null);
        [InspectorName("_TextureSample0_ST")]public Vector4Parameter _TextureSample0_ST = new(new Vector4(1,1,0,0));
        [InspectorName("极坐标方向")]public PolayEnumParameter _Float31 = new(0);
        [InspectorName("黑白闪流动控制方式")]public BWMoveEnumParameter _Float32 = new(0);
        [InspectorName("黑白闪纹理强度")]public ClampedFloatParameter _Float21 = new(0,0,1);
        
        [InspectorName("黑白范围")]public ClampedFloatParameter _Float2 = new(1,0,1);
        [InspectorName("黑白过度")]public ClampedFloatParameter _Float3 = new(0,0,0.1f);
       
        [Space(5),Header("震频__________________________________________________")]
        [InspectorName("震屏测试(Custom2xy_UV震频,zw_UV振幅)")]public ShakeEnumParameter _Float25 = new(0);
        [InspectorName("U震频测试(随数值变化往复震动)")]public FloatParameter _Float15 = new(0);
        [InspectorName("U振幅测试")]public ClampedFloatParameter _Float14 = new(0,0,1);
        [InspectorName("V振频测试(随数值变化往复震动)")]public FloatParameter _Float12 = new(0);
        [InspectorName("V振幅测试")]public ClampedFloatParameter _Float13 = new(0,0,1);

        [Space(5),Header("肌理__________________________________________________")]
        [InspectorName("肌理图")]public TextureParameter _TextureSample2 = new(null);
        [InspectorName("_TextureSample2_ST")]public Vector4Parameter _TextureSample2_ST = new(new Vector4(1,1,0,0));
        [InspectorName("旋转肌理图")]public ClampedFloatParameter _Float26 = new(0,0,1);
        [InspectorName("肌理强度")]public ClampedFloatParameter _Float23 = new(0,-1,1);

        public override string GetShaderName()
        {
            return  "Hidden/PostProcessing/Skill/BlackWhiteURP";
        }
    }

    public class BlackWhite3Renderer : VolumeRenderer<BlackWhite3>
    {
        //public override string ShaderName => "Hidden/PostProcessing/Skill/BlackWhite";
        public const string PROFILER_TAG = "BlackWhiteURP";

        static class ShaderIDs
        {
            //色阶
            public static readonly int _Float20 = Shader.PropertyToID("_Float20");
            public static readonly int _Float19 = Shader.PropertyToID("_Float19");
            public static readonly int _Float28 = Shader.PropertyToID("_Float28");
            public static readonly int _Float36 = Shader.PropertyToID("_Float36");
            public static readonly int _Float6 = Shader.PropertyToID("_Float6");
            public static readonly int _Float7 = Shader.PropertyToID("_Float7");
            
            //Mask
            public static readonly int _TextureSample1 = Shader.PropertyToID("_TextureSample1");
            public static readonly int _Float30 = Shader.PropertyToID("_Float30");
            public static readonly int _Float24 = Shader.PropertyToID("_Float24");
            public static readonly int _MaskPower1 = Shader.PropertyToID("_MaskPower1");
            public static readonly int _Float9 = Shader.PropertyToID("_Float9");
            
            //色散
            public static readonly int _Float11 = Shader.PropertyToID("_Float11");
            public static readonly int _Float17 = Shader.PropertyToID("_Float17");
            public static readonly int _Float5 = Shader.PropertyToID("_Float5");
            public static readonly int _Float18 = Shader.PropertyToID("_Float18");
            public static readonly int _Float8 = Shader.PropertyToID("_Float8");
            public static readonly int _Float27 = Shader.PropertyToID("_Float27");
            
            //黑白闪
            public static readonly int _Float4 = Shader.PropertyToID("_Float4");
            public static readonly int _Float10 = Shader.PropertyToID("_Float10");
            public static readonly int _Color0 = Shader.PropertyToID("_Color0");
            public static readonly int _Color1 = Shader.PropertyToID("_Color1");
            public static readonly int _Float33 = Shader.PropertyToID("_Float33");
            public static readonly int _Float34 = Shader.PropertyToID("_Float34");
            public static readonly int _TextureSample0 = Shader.PropertyToID("_TextureSample0");
            public static readonly int _Float31 = Shader.PropertyToID("_Float31");
            public static readonly int _Float32 = Shader.PropertyToID("_Float32");
            public static readonly int _Float21 = Shader.PropertyToID("_Float21");
            public static readonly int _TextureSample0_ST = Shader.PropertyToID("_TextureSample0_ST");
            public static readonly int _Float2 = Shader.PropertyToID("_Float2");
            public static readonly int _Float3 = Shader.PropertyToID("_Float3");
            public static readonly int _TextureSample1_ST = Shader.PropertyToID("_TextureSample1_ST");
            
            //震屏
            public static readonly int _Float25 = Shader.PropertyToID("_Float25");
            public static readonly int _Float15 = Shader.PropertyToID("_Float15");
            public static readonly int _Float14 = Shader.PropertyToID("_Float14");
            public static readonly int _Float12 = Shader.PropertyToID("_Float12");
            public static readonly int _Float13 = Shader.PropertyToID("_Float13");
            
            //肌理
            public static readonly int _TextureSample2 = Shader.PropertyToID("_TextureSample2");
            public static readonly int _Float26 = Shader.PropertyToID("_Float26");
            public static readonly int _Float23 = Shader.PropertyToID("_Float23");
            public static readonly int _TextureSample2_ST = Shader.PropertyToID("_TextureSample2_ST");
        }

        public override void Render(CommandBuffer cmd, 
            RenderTargetIdentifier source,
            RenderTargetIdentifier target,
            ref RenderingData renderingData)
        {

            //色阶
            m_BlitMaterial.SetFloat(ShaderIDs._Float20, settings._Float20.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float19, settings._Float19.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float28, settings._Float28.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float36, settings._Float36.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float6, settings._Float6.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float7, settings._Float7.value);
            
            //Mask
            m_BlitMaterial.SetTexture(ShaderIDs._TextureSample1, settings._TextureSample1.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float30, (float)settings._Float30.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float24, settings._Float24.value ? 1 : 0);
            m_BlitMaterial.SetFloat(ShaderIDs._MaskPower1, settings._MaskPower1.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float9, settings._Float9.value);
            
            //色散模糊
            m_BlitMaterial.SetFloat(ShaderIDs._Float11, settings._Float11.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float17, (float) settings._Float17.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float5, settings._Float5.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float18, (float)settings._Float18.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float8, settings._Float8.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float27, settings._Float27.value);
            
            //黑白闪
            m_BlitMaterial.SetFloat(ShaderIDs._Float4, settings._Float4.value ? 1:0);
            m_BlitMaterial.SetFloat(ShaderIDs._Float10, (float)settings._Float10.value);
            m_BlitMaterial.SetColor(ShaderIDs._Color0, settings._Color0.value);
            m_BlitMaterial.SetColor(ShaderIDs._Color1, settings._Color1.value);
            
            m_BlitMaterial.SetFloat(ShaderIDs._Float33, settings._Float33.value? 1: 0);
            m_BlitMaterial.SetFloat(ShaderIDs._Float34, (float)settings._Float34.value);
            m_BlitMaterial.SetTexture(ShaderIDs._TextureSample0, settings._TextureSample0.value);
            
            m_BlitMaterial.SetFloat(ShaderIDs._Float31, (float)settings._Float31.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float32, (float)settings._Float32.value);
            m_BlitMaterial.SetVector(ShaderIDs._TextureSample0_ST, settings._TextureSample0_ST.value);
            
            m_BlitMaterial.SetVector(ShaderIDs._TextureSample0_ST, settings._TextureSample0_ST.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float2, settings._Float2.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float3, settings._Float3.value);
            m_BlitMaterial.SetVector(ShaderIDs._TextureSample1_ST, settings._TextureSample1_ST.value);
            
            //震屏
            m_BlitMaterial.SetFloat(ShaderIDs._Float25, (float)settings._Float25.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float15, settings._Float15.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float14, settings._Float14.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float12, settings._Float12.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float13, settings._Float13.value);
            
            //肌理
            m_BlitMaterial.SetTexture(ShaderIDs._TextureSample2, settings._TextureSample2.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float26, settings._Float26.value);
            m_BlitMaterial.SetFloat(ShaderIDs._Float23, settings._Float23.value);
            m_BlitMaterial.SetVector(ShaderIDs._TextureSample2_ST, settings._TextureSample2_ST.value);
            
            cmd.Blit(source, target, m_BlitMaterial, 0);
        }

    }
}
