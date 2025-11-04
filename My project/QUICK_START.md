# 🎮 Quick Start: Complete Your Maze

## Step 1: Analyze Your Current Maze (5 minutes)

1. **Open your Unity scene** (`SampleScene.unity`)

2. **Create an empty GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "MazeTools"

3. **Add the Maze Analyzer**:
   - Select "MazeTools" in Hierarchy
   - In Inspector, click "Add Component"
   - Type "MazeAnalyzer" and select it

4. **Run the Analysis**:
   - Look at the Inspector for MazeTools
   - Click the button **"🔍 Analyze Existing Walls"**
   - Check the Console (Window → General → Console)
   - You'll see output like:
     ```
     Total walls found: 189
     Maze Bounds: Min: (0, 0, 0) Max: (34, 0, 24)
     Detected grid spacing: 1.0 units
     Suggested Start Offset: (0, 0, 0)
     Suggested Cell Size: 1.0
     ```
   - **Write down** the "Suggested Start Offset" and "Suggested Cell Size"

## Step 2: Generate Missing Walls (5 minutes)

1. **Add the Maze Generator** to the same GameObject:
   - With "MazeTools" selected
   - Add Component → "MazeFromImageGenerator"

2. **Configure the Generator**:
   - **Maze Texture**: 
     - In Project panel, go to Assets/Textures
     - Drag "35 by 25 orthogonal maze.png" to the "Maze Texture" field
   
   - **Cell Size**: Use the value from Step 1 (probably 1.0)
   
   - **Start Offset**: Use the values from Step 1 (probably 0, 0, 0)
   
   - **Skip Existing Walls**: ✓ Keep this checked
   
   - **Wall Prefab**: Leave empty (will create cubes) OR:
     - If you want matching walls, drag one of your existing walls here

3. **Make Sure Texture Import Settings Allow Reading**:
   - Select "35 by 25 orthogonal maze.png" in Project
   - In Inspector, under "Advanced":
   - Check "Read/Write Enabled"
   - Click "Apply"

3.5. **About Colored Solution Lines** ⭐:
   - If your image has a red solution line, **don't worry!**
   - The tool automatically detects and ignores colored pixels
   - Keep "Ignore Colored Pixels" checked (it's on by default)
   - See `COLORED_LINES_GUIDE.md` for details

4. **Generate!**:
   - Click the green button **"Generate Missing Walls from Image"**
   - Click "Yes, Generate"
   - Wait a moment...
   - Check Console for completion message
   - Your maze is now complete! 🎉

## Step 3: Review and Adjust (5 minutes)

1. **Look at Your Scene**:
   - New walls should appear where they were missing
   - They'll be organized under "GeneratedWalls" in Hierarchy

2. **If Walls Are Misaligned**:
   - Select "MazeTools"
   - Adjust "Start Offset" values (X, Y, Z)
   - Click "Clear Generated Walls"
   - Click "Generate Missing Walls from Image" again

3. **If Walls Are Wrong Size**:
   - Adjust "Cell Size"
   - Clear and regenerate

4. **If Walls Are Still Being Placed Over Existing Ones**:
   - Increase "Existing Wall Check Radius" from 0.3 to 0.5
   - Regenerate

## Troubleshooting

### Problem: "No walls found in the scene!"
- Make sure your existing walls have "Wall" in their name
- Or adjust the "Wall Name Contains" field in MazeAnalyzer

### Problem: "Maze texture is not assigned!"
- Make sure you dragged the PNG file to the Maze Texture slot
- Check that the file is in Assets/Textures folder

### Problem: Walls are at completely wrong positions
- Your reference image might be rotated
- Try adjusting Start Offset by full units (try adding/subtracting 1 or 2)
- The image coordinates might be flipped - this is normal, just adjust the offset

### Problem: Generated walls don't match existing wall style
- Create a prefab from one of your existing walls:
  1. Select an existing wall in Hierarchy
  2. Drag it to the Project panel (Assets folder)
  3. Delete the one you just dragged (keep the original)
  4. Assign the prefab to "Wall Prefab" field
  5. Regenerate

## What to Expect

Based on your maze being **35×25**:
- Total possible positions: **875**
- If you've manually placed ~189 walls
- The generator should create ~300-500 more walls
- Generation should take 5-30 seconds depending on your computer

## Tips

- **Save your scene before generating!** (Ctrl+S / Cmd+S)
- If you're not sure, use "Show Preview" and look for red wireframes in Scene view
- You can always click "Clear Generated Walls" and try again
- The analyzer's "Create Visualization Spheres" can help you see your current wall positions

---

## All Done? 🎉

Once your maze is complete:
1. Save your scene
2. You can delete the "MazeTools" GameObject if you want
3. Or keep it in case you need to regenerate
4. Test your maze!

**Need more help?** Check `MAZE_COMPLETION_GUIDE.md` for detailed information.
