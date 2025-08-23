
PS1 Cool Arrow Models
=====================

Files:
- arrow_up_cool.obj
- arrow_down_cool.obj
- ps1_arrow_materials.mtl
- arrow_body_ps1.png
- arrow_detail_ps1.png
- arrow_glow_on_ps1.png
- arrow_glow_off_ps1.png

Geometry:
- Low-poly ribs on the stem, small bolt nubs, layered head for a faux-bevel.
- Separate ArrowGlow mesh for easy ON/OFF scripting. Slightly offset to avoid z-fighting.

Unity Import:
- Textures: Max Size 128–256, Filter Mode=Point, Compression=None.
- Materials:
  Arrow_Body -> Base Map arrow_body_ps1.png, Metallic 0.5–0.6, Smoothness 0.2.
  Arrow_Detail -> Base Map arrow_detail_ps1.png, Metallic 0.4, Smoothness 0.15.
  Arrow_Glow_ON -> Emission ON, Emission Map arrow_glow_on_ps1.png.
  Arrow_Glow_OFF -> Emission OFF or use arrow_glow_off_ps1.png.
- Assign submeshes by object block order: Stem+Head = arrow_body, Bolts = arrow_detail, Glow = glow.

Pro tips:
- Disable texture filtering and AA for crunchy PS1 look.
- Optional: add slight vertex jitter in a shader for wobble.
